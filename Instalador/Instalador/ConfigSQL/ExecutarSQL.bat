@echo off
echo Executando script %1 ...

start /min sqlcmd -E -S localhost\NewSharp -i %1 -o %1.log

echo Script concluido!