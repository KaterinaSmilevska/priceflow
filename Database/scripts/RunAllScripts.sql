/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
:r .\LoadTipHV.sql
:r .\LoadUlogi.sql
:r .\LoadSektori.sql
:r .\LoadBrokeri.sql
:r .\LoadAplikativniParametri.sql
:r .\LoadKorisnici.sql
:r .\LoadKorisniciUlogi.sql
:r .\LoadIzdavachi.sql
:r .\LoadHartiiOdVrednost.sql
:r .\LoadPortfolija.sql
:r .\LoadFinansiskiPokazateli.sql
:r .\LoadPortfolioPrinosi.sql
:r .\LoadTransakcii.sql