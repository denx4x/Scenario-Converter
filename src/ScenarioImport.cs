using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using System.Windows.Forms;

public static class ScenarioImport { public static Dictionary<string,string> ClassifyDrop(string[] files){
  if(files==null||files.Length<1||files.Length>2)throw new Exception("Tarik satu JSON, satu editor, atau kedua file sekaligus.");
  var result=new Dictionary<string,string>();var serializer=new JavaScriptSerializer {MaxJsonLength=int.MaxValue,RecursionLimit=100};
  foreach(string file in files){if(!File.Exists(file))throw new Exception("Tarik file, bukan folder.");string ext=Path.GetExtension(file).ToLowerInvariant();if(ext!=".json"&&ext!=".editor"&&ext!=".txt")throw new Exception("File harus berformat .json atau .editor.");bool isEditor=ext==".editor";if(!isEditor){var raw=serializer.DeserializeObject(File.ReadAllText(file)) as Dictionary<string,object>;isEditor=raw!=null&&raw.ContainsKey("ListOfObjects");}string kind=isEditor?"editor":"json";if(result.ContainsKey(kind))throw new Exception("Pilih hanya satu file "+kind+" dalam sekali drop.");result.Add(kind,file);}
  return result;
 }
 public static Dictionary<string,string> Prepare(string jsonText,string editorText,Dictionary<string,object> current,bool replaceObjects,bool replaceType){
  var serializer=new JavaScriptSerializer {MaxJsonLength=int.MaxValue,RecursionLimit=100};
  object raw=serializer.DeserializeObject(jsonText),editor=editorText==null?null:serializer.DeserializeObject(editorText);
  var converted=raw as Dictionary<string,object>;bool hasObjects=converted!=null&&converted.ContainsKey("scenarioobject");
  if(replaceObjects&&editor==null&&!hasObjects)throw new Exception("Pilih file .editor untuk mengganti object list, atau matikan opsi ganti object list.");
  object fallback=editor??(hasObjects?null:current);
  var built=(Dictionary<string,object>)KonverterSkenario.Build(raw,fallback,"","","","","");
  var edits=new Dictionary<string,string>{{"scenario",ContentDocument.FormatJson(serializer.Serialize(built["scenario"]))}};
  if(replaceObjects)edits.Add("scenarioobject",ContentDocument.FormatJson(serializer.Serialize(built["scenarioobject"])));
  if(replaceType)edits.Add("type",ContentDocument.FormatJson(serializer.Serialize(built["type"])));
  return edits;
 }
}

public partial class ContentEditorForm {
 readonly Dictionary<string,string> pendingImport=new Dictionary<string,string>();
 void ImportScenario(){if(selected<0){MessageBox.Show(this,"Pilih content terlebih dahulu.");return;}using(var dialog=new ScenarioImportDialog(Value(document.Item(selected),"id"))){if(dialog.ShowDialog(this)!=DialogResult.OK)return;try{StageImport(dialog.JsonPath,dialog.EditorPath,dialog.ReplaceObjects,dialog.ReplaceType);}catch(Exception ex){MessageBox.Show(this,ex.Message,"Import gagal",MessageBoxButtons.OK,MessageBoxIcon.Error);}}}
 void StageImport(string jsonFile,string editorFile,bool replaceObjects,bool replaceType){
  var edits=ScenarioImport.Prepare(File.ReadAllText(jsonFile),string.IsNullOrWhiteSpace(editorFile)?null:File.ReadAllText(editorFile),document.Item(selected),replaceObjects,replaceType);
  foreach(string key in edits.Keys)if(document.FieldRaw(selected,key)==null)throw new Exception("Content terpilih tidak memiliki field "+key+". Tidak ada perubahan diterapkan.");
  ValidateScenario(edits["scenario"]);
  pendingImport.Clear();foreach(var pair in edits)if(pair.Key!="scenario")pendingImport.Add(pair.Key,pair.Value);
  scenario.Text=edits["scenario"];pending=true;
  status.Text="Import siap untuk content ID "+Value(document.Item(selected),"id")+". Object list: "+(replaceObjects?"diganti":"dipertahankan")+"; type: "+(replaceType?"diperbarui":"dipertahankan")+". Klik Terapkan atau Simpan sebagai.";
 }
}

public sealed class ScenarioImportDialog : Form {
 readonly TextBox jsonPath=new TextBox(),editorPath=new TextBox();
 readonly CheckBox objects=new CheckBox {Text="Ganti scenarioobject dari editor / JSON impor",Checked=true,AutoSize=true};
 readonly CheckBox types=new CheckBox {Text="Perbarui type mengikuti skenario impor",Checked=false,AutoSize=true};
 public string JsonPath {get{return jsonPath.Text;}}public string EditorPath {get{return editorPath.Text;}}public bool ReplaceObjects {get{return objects.Checked;}}public bool ReplaceType {get{return types.Checked;}}
 public ScenarioImportDialog(string id){Text="Import pengganti scenario · Content ID "+id;Size=new Size(820,400);MinimumSize=new Size(700,400);StartPosition=FormStartPosition.CenterParent;Font=new Font("Segoe UI",10);Icon=Icon.ExtractAssociatedIcon(Application.ExecutablePath);var root=new TableLayoutPanel {Dock=DockStyle.Fill,ColumnCount=1,RowCount=8,Padding=new Padding(14)};Controls.Add(root);root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));
  Add(root,new Label {Text="Tarik JSON dan editor ke jendela ini (boleh sekaligus). Target: content ID "+id+".",AutoSize=true},38);
  Add(root,new Label {Text="JSON skenario baru",AutoSize=true},24);Add(root,FileRow(jsonPath,false),36);
  Add(root,new Label {Text="Editor object list (opsional jika object list tidak diganti / sudah ada di JSON)",AutoSize=true},24);Add(root,FileRow(editorPath,true),36);Add(root,objects,30);Add(root,types,30);
  var buttons=new FlowLayoutPanel {Dock=DockStyle.Fill};var import=new Button {Text="Import pengganti langkah",AutoSize=true};var cancel=new Button {Text="Batal",DialogResult=DialogResult.Cancel,AutoSize=true};buttons.Controls.Add(import);buttons.Controls.Add(cancel);Add(root,buttons,45);CancelButton=cancel;import.Click+=(s,e)=>{if(!File.Exists(jsonPath.Text)){MessageBox.Show(this,"Pilih JSON skenario yang valid.");return;}if(editorPath.Text!=""&&!File.Exists(editorPath.Text)){MessageBox.Show(this,"File editor tidak ditemukan.");return;}DialogResult=DialogResult.OK;};EnableFileDrop(this);}
 int nextRow;void Add(TableLayoutPanel p,Control c,int height){p.RowStyles.Add(new RowStyle(SizeType.Absolute,height));c.Dock=DockStyle.Fill;p.Controls.Add(c,0,nextRow++);}
 Control FileRow(TextBox box,bool editor){var row=new TableLayoutPanel {ColumnCount=2,Dock=DockStyle.Fill};row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent,100));row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute,85));box.Dock=DockStyle.Fill;var button=new Button {Text="Pilih…",Dock=DockStyle.Fill};row.Controls.Add(box,0,0);row.Controls.Add(button,1,0);button.Click+=(s,e)=>{using(var d=new OpenFileDialog {Filter=editor?"Editor|*.editor;*.json;*.txt|Semua file|*.*":"JSON|*.json"})if(d.ShowDialog()==DialogResult.OK)box.Text=d.FileName;};return row;}
 void DropFiles(string[] files){var paths=ScenarioImport.ClassifyDrop(files);string file;if(paths.TryGetValue("json",out file))jsonPath.Text=file;if(paths.TryGetValue("editor",out file))editorPath.Text=file;}
 void EnableFileDrop(Control control){control.AllowDrop=true;control.DragEnter+=(s,e)=>{e.Effect=e.Data.GetDataPresent(DataFormats.FileDrop)?DragDropEffects.Copy:DragDropEffects.None;};control.DragDrop+=(s,e)=>{try{DropFiles(e.Data.GetData(DataFormats.FileDrop) as string[]);}catch(Exception ex){MessageBox.Show(this,ex.Message,"File tidak dapat dimuat",MessageBoxButtons.OK,MessageBoxIcon.Error);}};foreach(Control child in control.Controls)EnableFileDrop(child);}
}
