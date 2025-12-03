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
:r .\LoadHub_Brokeri.sql
:r .\LoadHub_HartiiOdVrednost.sql
:r .\LoadHub_Izdavachi.sql
:r .\LoadHub_Korisnici.sql
:r .\LoadHub_Portfolija.sql
:r .\LoadHub_Sektori.sql
:r .\LoadHub_TipHV.sql
:r .\LoadHub_Ulogi.sql
:r .\LoadLink_HartiiOdVrednost_Izdavachi.sql
:r .\LoadLink_HartiiOdVrednost_TipHV.sql
:r .\LoadLink_Izdavachi_Sektori.sql
:r .\LoadLink_KorisniciUlogi.sql
:r .\LoadLink_Portfolija_Korisnici.sql
:r .\LoadLink_PortfolioPrinosi.sql
:r .\LoadLink_Transakcii.sql