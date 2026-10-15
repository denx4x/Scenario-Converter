using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class KonverterSkenario {
 const string Repository="denx4x/Scenario-Converter";
 static string CurrentVersion { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(3); } }
 static Version ParseVersion(string tag){Version version;if(!Version.TryParse(tag.TrimStart('v','V'),out version))throw new Exception("Nomor versi rilis tidak valid.");return version;}
 static string HashFile(string path){using(var hash=SHA256.Create())using(var stream=File.OpenRead(path))return BitConverter.ToString(hash.ComputeHash(stream)).Replace("-","").ToLowerInvariant();}
 static string ReleaseAsset(object release,string name){var assets=Get(release,"assets") as object[];if(assets!=null)foreach(var a in assets)if(Str(Get(a,"name"))==name){string url=Str(Get(a,"browser_download_url"));if(!url.StartsWith("https://github.com/"+Repository+"/releases/download/",StringComparison.OrdinalIgnoreCase))throw new Exception("Alamat aset rilis tidak valid.");return url;}throw new Exception("Rilis belum memuat "+name+".");}
 async Task CheckForUpdates(Button button){
  button.Enabled=false;string folder=null;bool installing=false;
  try {
   status.Text="Memeriksa GitHub Releases…";ServicePointManager.SecurityProtocol|=SecurityProtocolType.Tls12;
   using(var web=new WebClient()){
    web.Headers[HttpRequestHeader.UserAgent]="Scenario-Converter/"+CurrentVersion;
    object release;
    try{release=json.DeserializeObject(await web.DownloadStringTaskAsync("https://api.github.com/repos/"+Repository+"/releases/latest"));}
    catch(WebException ex){var response=ex.Response as HttpWebResponse;if(response!=null&&response.StatusCode==HttpStatusCode.NotFound){status.Text="Belum ada rilis publik. Versi terpasang: "+CurrentVersion;return;}throw;}
    Version latest=ParseVersion(Str(Get(release,"tag_name")));
    if(latest.CompareTo(ParseVersion(CurrentVersion))<=0){status.Text="Versi "+CurrentVersion+" sudah terbaru.";return;}
    string asset=ReleaseAsset(release,"Scenario-Converter.exe"),checksum=ReleaseAsset(release,"Scenario-Converter.exe.sha256");
    string notes=Str(Get(release,"body"));if(notes.Length>1400)notes=notes.Substring(0,1400)+"…";
    if(MessageBox.Show(this,"Versi "+latest+" tersedia.\n\n"+notes+"\n\nSimpan pekerjaan Anda sebelum melanjutkan. Aplikasi akan ditutup dan dibuka kembali setelah update.\n\nPasang pembaruan?","Pembaruan Scenario Converter",MessageBoxButtons.YesNo,MessageBoxIcon.Information)!=DialogResult.Yes)return;
    folder=Path.Combine(Path.GetTempPath(),"ScenarioConverter-"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(folder);
    string package=Path.Combine(folder,"Scenario-Converter.exe");
    status.Text="Mengunduh pembaruan…";
    await web.DownloadFileTaskAsync(asset,package);
    string expected=(await web.DownloadStringTaskAsync(checksum)).Trim().Split(new[]{' ','\t','\r','\n'},StringSplitOptions.RemoveEmptyEntries)[0].ToLowerInvariant();
    if(expected.Length!=64||HashFile(package)!=expected)throw new Exception("Checksum pembaruan tidak cocok. Update dibatalkan.");
    if(ParseVersion(AssemblyName.GetAssemblyName(package).Version.ToString(3)).CompareTo(latest)!=0)throw new Exception("Versi file pembaruan tidak sesuai dengan tag rilis.");
    string helper=Path.Combine(folder,"ScenarioConverter.Update.exe");
    using(var resource=Assembly.GetExecutingAssembly().GetManifestResourceStream("ScenarioConverter.Updater.exe"))using(var output=File.Create(helper)){if(resource==null)throw new Exception("Komponen updater tidak tersedia.");resource.CopyTo(output);}
    Process.Start(new ProcessStartInfo(helper,"\""+Process.GetCurrentProcess().Id+"\" \""+Application.ExecutablePath+"\" \""+package+"\" \""+expected+"\""){UseShellExecute=false,CreateNoWindow=true});
    installing=true;Application.Exit();
   }
  }catch(Exception ex){status.Text="Pemeriksaan/update gagal: "+ex.Message;MessageBox.Show(this,status.Text,"Pembaruan",MessageBoxButtons.OK,MessageBoxIcon.Error);}
  finally{button.Enabled=true;if(!installing&&folder!=null)try{Directory.Delete(folder,true);}catch{}}
 }
}

