# Scenario Converter

Aplikasi desktop Windows untuk mengonversi dan mengedit JSON skenario. Gabungkan skenario dengan object list editor, cari content, lalu ubah bagian yang diperlukan melalui pratinjau sebelum menyimpan hasilnya.

## Download

Unduh **Scenario-Converter.exe** pada bagian **Assets** di [rilis terbaru](https://github.com/denx4x/Scenario-Converter/releases/latest), lalu jalankan aplikasinya.

Gunakan Windows 10/11 dengan .NET Framework 4.8. Aplikasi dapat digunakan tanpa browser. File skenario diproses secara lokal; koneksi internet digunakan saat memeriksa dan mengunduh pembaruan.

## Konversi JSON dan editor

1. Pilih atau tarik file `.json` skenario dan file `.editor` ke jendela aplikasi. Kedua file boleh dijatuhkan sekaligus.
2. Isi atau edit **Nama content**, **Description**, **Chapter**, **Subchapter**, dan **PDF URL**.
3. Klik **Konversi** untuk melihat pratinjau.
4. Klik **Simpan JSON** untuk menyimpan hasil.

Jika JSON sudah memuat object list, file editor dapat dikosongkan.

## Membuka dan mengedit content

Fitur berikut tersedia mulai **v1.0.1**.

1. Klik **Buka / edit JSON content** dan pilih file yang akan diedit.
2. Cari berdasarkan **ID content**, **Chapter**, **Subchapter**, atau **Nama content**, lalu klik **Cari**. Kosongkan pencarian untuk menampilkan semua content.
3. Pilih content dari daftar hasil.
4. Gunakan tab **Informasi / Metadata** untuk mengedit informasi content, atau tab **scenario / Langkah JSON** untuk mengedit langkahnya.
5. Klik **Terapkan ke content terpilih**, lalu **Simpan sebagai…**.

**ID content** adalah nilai `id` pada content utama. Setiap langkah di dalam `scenario` memiliki ID tersendiri.

Perubahan diterapkan hanya pada field yang diedit dalam content terpilih. Content lain dan field tambahan dipertahankan. Aplikasi akan meminta keputusan jika Anda berpindah content dengan edit yang belum diterapkan.

Tombol **Batalkan edit content** mengembalikan edit yang belum diterapkan ke kondisi terakhir yang sudah diterapkan. Tombol ini tidak membatalkan perubahan yang sebelumnya sudah diterapkan.

## Mengganti langkah dari file lain

1. Pilih content tujuan, lalu buka tab **scenario / Langkah JSON**.
2. Klik **Import JSON + editor / Ganti langkah…**.
3. Pilih atau tarik JSON skenario baru dan file editor ke jendela import. Anda juga dapat menjatuhkan keduanya sekaligus ke area mana pun di jendela tersebut.
4. Tentukan opsi penggantian:
   - **Ganti scenarioobject**: memakai object list dari file editor atau JSON impor. Opsi ini aktif secara default. Matikan jika hanya ingin mengganti langkah.
   - **Perbarui type**: menyesuaikan daftar jenis interaksi dengan skenario yang diimpor. Opsi ini dapat diaktifkan sesuai kebutuhan.
5. Klik **Import pengganti langkah**, lalu periksa hasilnya di editor.
6. Klik **Terapkan ke content terpilih** dan **Simpan sebagai…**.

Jika JSON impor sudah dikonversi dan memuat `scenarioobject`, file editor tidak diperlukan. Nama content, description, chapter, subchapter, dan PDF URL tujuan tetap dipertahankan saat import.

Hasil import ditampilkan dengan indentasi. Gunakan **Rapikan JSON** untuk merapikan langkah yang sebelumnya ditulis dalam bentuk padat. `parameterscript` tetap berupa string JSON sesuai format skenario.

## Menyimpan hasil

Gunakan **Simpan sebagai…** untuk menentukan lokasi dan nama file hasil. Sebaiknya simpan sebagai file baru agar file sumber tetap tersedia.

Editor mendukung satu object content, array content `[]`, dan beberapa object content yang dipisahkan koma. Format sumber dipertahankan. Untuk daftar object tanpa pembungkus array, aktifkan **Ekspor sebagai array JSON standar []** jika aplikasi tujuan memerlukan satu dokumen JSON standar.

## Pembaruan aplikasi

Klik **Cek pembaruan** pada jendela utama. Jika tersedia versi lebih baru, aplikasi menampilkan informasi rilis dan meminta persetujuan sebelum mengunduh serta memasangnya.

Simpan pekerjaan terlebih dahulu karena aplikasi akan ditutup dan dibuka kembali saat pemasangan update. Pembaruan juga dapat diunduh dari [halaman Releases](https://github.com/denx4x/Scenario-Converter/releases).

## Jika mengalami masalah

- **Object list tidak ditemukan**: pilih editor yang sesuai, atau gunakan JSON hasil konversi yang sudah memuat `scenarioobject`.
- **JSON tidak valid**: periksa tanda kurung, tanda kutip, dan koma pada bagian yang diedit.
- **Fitur terbaru belum muncul**: tutup aplikasi lama dan jalankan versi terbaru yang sudah diunduh.
- **Pembaruan tidak tersedia**: fitur hanya membaca rilis publik yang sudah dipublikasikan.

Laporkan masalah melalui [Issues](https://github.com/denx4x/Scenario-Converter/issues) dengan langkah kejadian dan contoh data seperlunya.
