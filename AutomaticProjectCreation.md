# Introduction #

This page describes how to create projects using the automatic project builder.

# Details #

Simply run the program DataClassFileBuilder.exe and fill out the fields. If your data sources is file based, you can browse to it. After filling out the form, click "Convert DB to Classes". In the destination folder is a project with the entered name. Include this project in your solution file.

If you modify the database, just run the program again, and point at the same folder and the project will be updated. To avoid loosing data, do not modify code outside the "Unsynchronized Custom Code Region".