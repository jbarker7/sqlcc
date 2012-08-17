@echo off
cd /d %1
copy SQLCC.Impl.HtmlCodeHighlighter\bin\%2\SQLCC.Impl.HtmlCodeHighlighter.dll SQLCC\bin\%2\SQLCC.Impl.HtmlCodeHighlighter.dll
copy SQLCC.Impl.HtmlCodeHighlighter\bin\%2\Ionic.Zip.Reduced.dll SQLCC\bin\%2\Ionic.Zip.Reduced.dll
copy SQLCC.Impl.MsSqlProvider\bin\%2\SQLCC.Impl.MsSqlProvider.dll SQLCC\bin\%2\SQLCC.Impl.MsSqlProvider.dll
