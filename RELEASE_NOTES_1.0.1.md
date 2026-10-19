# Scenario Converter v1.0.1

- Buka JSON berisi satu atau banyak content untuk diedit.
- Cari berdasarkan ID content, chapter, subchapter, atau nama content.
- Edit metadata dan array `scenario` hanya pada content yang dipilih.
- Pertahankan field lain, field tambahan, urutan, dan teks content yang tidak diedit.
- Konfirmasi edit yang belum diterapkan sebelum berpindah content atau menutup jendela.
- Simpan hasil sebagai file baru, dengan opsi mengekspor daftar content menjadi array JSON standar.
- Dukung contoh format beberapa object yang dipisahkan koma, selain object tunggal dan array standar.

ID content adalah `id` pada tingkat content. ID langkah di dalam `scenario` tetap berada di editor langkah JSON.

- Import JSON skenario dan editor untuk mengganti langkah content terpilih, dengan opsi mengganti object list dan memperbarui type. Metadata serta content lain tetap dipertahankan.
