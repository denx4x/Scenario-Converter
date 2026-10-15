using System;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Windows.Forms;

class UpdateHelper {
 static string Hash(string path){using(var h=SHA256.Create())using(var f=File.OpenRead(path))return BitConverter.ToString(h.ComputeHash(f)).Replace("-","").ToLowerInvariant();}
 [STAThread] static void Main(string[] args){
  if(args.Length!=4)return;
  string target=Path.GetFullPath(args[1]),package=Path.GetFullPath(args[2]),backup=target+".previous",staged=target+".update";bool moved=false,installed=false;
  try {
   if(Hash(package)!=args[3])throw new Exception("Checksum tidak cocok.");
   try{var old=Process.GetProcessById(int.Parse(args[0]));if(!old.WaitForExit(30000))throw new Exception("Aplikasi belum ditutup. Coba update kembali.");}catch(ArgumentException){}
   File.Copy(package,staged,true);
   if(Hash(staged)!=args[3])throw new Exception("Verifikasi salinan gagal.");
   if(File.Exists(backup))File.Delete(backup);
   File.Move(target,backup);moved=true;
   File.Move(staged,target);installed=true;
   Process.Start(new ProcessStartInfo(target){UseShellExecute=true});
  }catch(Exception ex){
   try{if(moved){if(installed&&File.Exists(target))File.Delete(target);File.Move(backup,target);}if(File.Exists(staged))File.Delete(staged);}catch{}
   MessageBox.Show("Update gagal: "+ex.Message+"\nJalankan kembali aplikasi. File versi sebelumnya dipertahankan jika pemasangan gagal.","Scenario Converter",MessageBoxButtons.OK,MessageBoxIcon.Error);
  }
 }
}
