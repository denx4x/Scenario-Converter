# Scenario Converter

Aplikasi desktop Windows untuk mengonversi file JSON skenario dan object list editor ke format standar, dengan drag-and-drop, pengeditan metadata, pratinjau, dan pembaruan melalui GitHub Releases.

## Menggunakan aplikasi

1. Unduh `Scenario-Converter.exe` dari halaman **Releases** dan simpan di folder yang dapat Anda tulis.
2. Jalankan aplikasi, lalu pilih atau tarik file `.json` dan `.editor` ke jendela.
3. Edit nama content, description, chapter, subchapter, dan PDF URL.
4. Klik **Konversi**, periksa hasilnya, lalu **Simpan JSON**.

Gunakan **Buka JSON hasil konversi** untuk mengedit hasil yang sudah ada tanpa memilih editor lagi. File skenario diproses di komputer Anda. Koneksi internet hanya digunakan saat menekan **Cek pembaruan**.

Windows dengan .NET Framework 4.5 atau lebih baru diperlukan; aplikasi ditargetkan untuk Windows 10/11 dengan .NET Framework 4.8. Tidak perlu memasang Node.js atau membuka browser.

## Format hasil

Field utama: `id`, `chapter`, `subChapter`, `type`, `namaContent`, `description`, `scenario`, `scenarioobject`, dan `pdfurl`.

- `skenario.Items` menjadi `scenario`.
- `AsbunName` menjadi `namaContent`.
- `objectId` menjadi `GameObjectID`.
- Command awal `First` menjadi `Begin`, sedangkan `event_name` dipertahankan.
- `parameterscript` tetap string JSON; nilai kosong menjadi `null`.
- Mendukung editor yang menyimpan skenario dan object list sebagai string JSON.

## Build dari source

Jalankan dari PowerShell pada Windows:

```powershell
./build.ps1 -Version 1.0.0
```

Hasilnya ada di `bin/Scenario-Converter.exe` beserta `.sha256`. Compiler .NET Framework bawaan Windows digunakan. Folder `bin` tidak perlu diunggah ke Git.

## Menerbitkan pembaruan

Setelah source tersimpan di GitHub, buat tag versi baru:

```powershell
git tag v1.0.0
git push origin v1.0.0
```

Workflow GitHub Actions membangun aplikasi dan membuat **draft release** berisi EXE serta checksum. Periksa draft di GitHub, tulis catatan perubahan, lalu klik **Publish release**. Update berikutnya menggunakan versi yang lebih tinggi, misalnya `v1.0.1`. Tag harus berbentuk `vMAJOR.MINOR.PATCH`.

Tombol **Cek pembaruan** membaca rilis publik terbaru dari `denx4x/Scenario-Converter`. Aplikasi meminta persetujuan sebelum memasang, memverifikasi SHA-256 dan versi EXE, lalu membuka ulang aplikasi. Simpan edit sebelum menerima pembaruan. Versi sebelumnya disimpan sebagai `.exe.previous` agar pemasangan yang gagal dapat dipulihkan. Pemeriksaan checksum mendeteksi file yang rusak; itu bukan tanda tangan digital penerbit.

Sampai rilis pertama dipublikasikan, aplikasi akan menampilkan bahwa belum ada rilis publik. Repository privat tidak didukung oleh updater ini.

## Struktur source

- `src/ScenarioConverter.cs`: UI dan konversi.
- `src/Updates.cs`: pemeriksaan rilis dan pengunduhan update.
- `src/UpdateHelper.cs`: pemasangan update setelah aplikasi ditutup.
- `assets/app.ico`: ikon aplikasi.
- `.github/workflows/release.yml`: build dan draft release dari tag.
