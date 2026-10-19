using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Web.Script.Serialization;

// Keep original text outside an explicitly edited value, including unknown fields.
public sealed class ContentDocument {
 public sealed class Span { public int Start,End; public Span(int start,int end){Start=start;End=end;} }
 static readonly JavaScriptSerializer Json=new JavaScriptSerializer {MaxJsonLength=int.MaxValue,RecursionLimit=100};
 public string Text {get;private set;}
 public bool IsArray {get;private set;}
 public bool IsSequence {get {return !IsArray&&Parts.Count>1;}}
 public List<Span> Parts {get;private set;}
 public int Count {get{return Parts.Count;}}
 public ContentDocument(string text){Text=text;Index();}
 static void White(string text,ref int p){while(p<text.Length&&char.IsWhiteSpace(text[p]))p++;}
 static int StringEnd(string text,int p){p++;while(p<text.Length){if(text[p]=='\\'){p+=2;continue;}if(text[p++]=='"')return p;}throw new FormatException("String JSON belum ditutup.");}
 static int ValueEnd(string text,int p){if(p>=text.Length)throw new FormatException("Nilai JSON belum lengkap.");if(text[p]=='"')return StringEnd(text,p);if(text[p]=='{'||text[p]=='['){var stack=new Stack<char>();stack.Push(text[p]=='{'?'}':']');p++;while(p<text.Length&&stack.Count>0){char c=text[p];if(c=='"'){p=StringEnd(text,p);continue;}if(c=='{'||c=='[')stack.Push(c=='{'?'}':']');else if(c=='}'||c==']'){if(stack.Pop()!=c)throw new FormatException("Tanda penutup JSON tidak cocok.");}p++;}if(stack.Count>0)throw new FormatException("JSON belum ditutup.");return p;}int start=p;while(p<text.Length&&text[p]!=','&&text[p]!=']'&&text[p]!='}'&&!char.IsWhiteSpace(text[p]))p++;if(start==p)throw new FormatException("Nilai JSON tidak valid.");return p;}
 void Index(){Parts=new List<Span>();int p=0;White(Text,ref p);IsArray=p<Text.Length&&Text[p]=='[';if(IsArray){Json.DeserializeObject(Text);p++;White(Text,ref p);}while(p<Text.Length&&(!IsArray||Text[p]!=']')){if(Text[p]!='{')throw new FormatException("File cần chứa content JSON hoặc danh sách content.");int end=ValueEnd(Text,p);var obj=Json.DeserializeObject(Text.Substring(p,end-p)) as Dictionary<string,object>;if(obj==null||!obj.ContainsKey("scenario")||!(obj["scenario"] is object[]))throw new FormatException("Content phải chứa array scenario.");Properties(Text.Substring(p,end-p));Parts.Add(new Span(p,end));p=end;White(Text,ref p);if(IsArray&&p<Text.Length&&Text[p]==']')break;if(p==Text.Length)break;if(Text[p++]!=',')throw new FormatException("Thiếu dấu phẩy giữa content.");White(Text,ref p);if(p==Text.Length)throw new FormatException("Dấu phẩy thừa cuối file.");}if(IsArray){if(p>=Text.Length||Text[p++]!=']')throw new FormatException("Array chưa đóng.");White(Text,ref p);}if(p!=Text.Length||Parts.Count==0)throw new FormatException("File không chứa danh sách content hợp lệ.");}
 public string Raw(int index){var s=Parts[index];return Text.Substring(s.Start,s.End-s.Start);}
 public Dictionary<string,object> Item(int index){return (Dictionary<string,object>)Json.DeserializeObject(Raw(index));}
 public static Dictionary<string,Span> Properties(string raw){var result=new Dictionary<string,Span>();int p=0;White(raw,ref p);if(raw[p++]!='{')throw new FormatException("Content phải là object.");White(raw,ref p);while(p<raw.Length&&raw[p]!='}'){if(raw[p]!='"')throw new FormatException("Tên field không hợp lệ.");int end=StringEnd(raw,p);string key=(string)Json.DeserializeObject(raw.Substring(p,end-p));p=end;White(raw,ref p);if(raw[p++]!=':')throw new FormatException("Thiếu dấu hai chấm.");White(raw,ref p);int start=p;p=ValueEnd(raw,p);if(result.ContainsKey(key))throw new FormatException("Field trùng lặp: "+key);result.Add(key,new Span(start,p));White(raw,ref p);if(raw[p]=='}')break;if(raw[p++]!=',')throw new FormatException("Thiếu dấu phẩy.");White(raw,ref p);}return result;}
 public string FieldRaw(int index,string key){string raw=Raw(index);Span span;if(!Properties(raw).TryGetValue(key,out span))return null;return raw.Substring(span.Start,span.End-span.Start);}
 public void Patch(int index,Dictionary<string,string> edits){string raw=Raw(index);var properties=Properties(raw);var replacements=new List<KeyValuePair<Span,string>>();foreach(var pair in edits){Span span;if(!properties.TryGetValue(pair.Key,out span))throw new FormatException("Content không có field "+pair.Key+". Không tự thêm field.");object value=Json.DeserializeObject(pair.Value);bool arrayField=pair.Key=="scenario"||pair.Key=="scenarioobject"||pair.Key=="type";if(arrayField&&!(value is object[]))throw new FormatException("scenario phải là array JSON.");if(!arrayField&&!(value is string))throw new FormatException(pair.Key+" phải là chuỗi.");replacements.Add(new KeyValuePair<Span,string>(span,pair.Value));}replacements.Sort((a,b)=>b.Key.Start.CompareTo(a.Key.Start));foreach(var r in replacements)raw=raw.Substring(0,r.Key.Start)+r.Value+raw.Substring(r.Key.End);Json.DeserializeObject(raw);var part=Parts[index];string candidate=Text.Substring(0,part.Start)+raw+Text.Substring(part.End);var verified=new ContentDocument(candidate);Text=verified.Text;Parts=verified.Parts;IsArray=verified.IsArray;}
 public List<int> Search(string field,string query){var found=new List<int>();string[] keys=field=="all"?new[]{"id","chapter","subChapter","namaContent"}:new[]{field};for(int i=0;i<Count;i++){var item=Item(i);foreach(var key in keys){object value;item.TryGetValue(key,out value);if(Convert.ToString(value).IndexOf(query,StringComparison.OrdinalIgnoreCase)>=0){found.Add(i);break;}}}return found;}
 public static string FormatJson(string text){
  Json.DeserializeObject(text);var output=new StringBuilder();int depth=0;
  for(int p=0;p<text.Length;p++){
   char c=text[p];if(char.IsWhiteSpace(c))continue;
   if(c=='"'){int end=StringEnd(text,p);output.Append(text,p,end-p);p=end-1;continue;}
   if(c=='{'||c=='['){output.Append(c);int next=p+1;White(text,ref next);char close=c=='{'?'}':']';if(next<text.Length&&text[next]==close){output.Append(close);p=next;continue;}depth++;output.AppendLine();output.Append(' ',depth*2);}
   else if(c=='}'||c==']'){depth--;output.AppendLine();output.Append(' ',depth*2);output.Append(c);}
   else if(c==','){output.Append(c);output.AppendLine();output.Append(' ',depth*2);}
   else if(c==':')output.Append(": ");else output.Append(c);
  }
  return output.ToString();
 }
 public static string Encode(string text){return Json.Serialize(text);}
 public string Export(bool standardArray){return standardArray&&!IsArray?"["+Environment.NewLine+Text+Environment.NewLine+"]":Text;}
}
