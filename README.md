##### Integratieproject 1 - Academiejaar 2023-2024

# Phygital Applicatie

## Algemene informatie

### Team 17: Programmers in Paris

#### Teamleden:
- **Marwane Ben Ali**  
  marwane.benali@student.kdg.be  
  Project Manager (SPOC)
  
- **Wassim Fouad**  
  wassim.fouad@student.kdg.be  
  Developer & Quality Manager
  
- **Bilal Hafa**  
  bilal.hafa@student.kdg.be  
  Developer & Data Scientist
  
- **Marouane Oulad El Arbi**  
  marouane.ouladelarbi@student.kdg.be  
  Developer & Software Engineer
  
- **Matthias Adriaenssen**  
  matthias.adriaenssen@student.kdg.be  
  Deployment Manager

#### Korte Omschrijving van het Project:
We hebben een applicatie ontwikkeld om jongeren te betrekken bij lokale verkiezingen via een combinatie van fysieke en digitale tools ("Phygital"). De tool biedt informatie over verkiezingen en verzamelt jongeren hun meningen door interactieve installaties en een webapplicatie. Er zijn verschillende gebruikersrollen: jongeren, begeleiders, lokale overheden, en platformbeheerders. De applicatie is gebruiksvriendelijk, toegankelijk, en voldoet aan beveiligings- en prestatievereisten. Testing en feedback zijn integraal om de werking en aantrekkelijkheid te verbeteren.
Voor de fysieke implementatie in het openbaar waarop de draaiende applicatie toegangelijk is, is er een kiosk die we ontworpen hebben.
Bovendien kunnen gebruikers in bepaalde situaties van het doorlopen van de applicatie, meevolgen door QR-codes te scannen met hun smartphone.

## Dotnet repository

### Instructies om het Project te Compileren:

##### BINNEN DE DOTNET FOLDER
```bash
npm install
npm run build
dotnet restore ./UI.MVC/UI.MVC.csproj
dotnet build  ./UI.MVC/UI.MVC.csproj --configuration Release
```
### Instructies om het Project te Runnen na de build:

##### BINNEN DE DOTNET FOLDER
```bash
dotnet publish ./UI.MVC/UI.MVC.csproj --configuration Release --output ../phygitalappbuilt
dotnet ../phygitalappbuilt/UI.MVC.dll --urls http://*:8080
```
### Credentials van Testgebruikers:

- **Gebruiker:** admin@kdg.be  
  **Wachtwoord:** 

- **Gebruiker:** Sadmin@kdg.be  
  **Wachtwoord:** 

- **Gebruiker:** companion@kdg.be  
  **Wachtwoord:**

### Bijkomende informatie voor development

Het lokaal runnen van de applicatie zal automatisch werken met een sqlite databank. Om te runnen met de PostgreSQL databank die gedeployed staat in de cloud, zult u in dotnet/UI.MVC/Properties/launchSettings.json de ASPNETCORE_ENVIROMENT moeten aanpassen van "Development" naar "Production".

Er worden GEEN belangrijke credentials meegegeven in deze repositories.

## Deployment Repository

Dit document bevat uitgebreide instructies voor het deployen van het project, inclusief scripts en aanvullende uitleg over verschillende onderdelen. 

### Deploy Script Uitleg

Het `deploy.sh` script is verantwoordelijk voor het implementeren van de applicatie op Google Cloud. Het configureert verschillende services en voorziet in de benodigde infrastructuur. 

De applicatie wordt gedeployed via deze script op een linux machine, maar deze linux machine zal zelf de applicatie niet hosten. De google cloud compute engine omgeving samen met andere modules in de google cloud omgeving zal de applicatie online en in stand houden.

Bovendien is er in de fysieke kiosk zelf een jetson nano board nodig, met een infrarood sensor en makey-makey apparatuur voor het bedienen van de kiosk door gebruikers.

#### Instructies en Prerequisites

Voordat u het script uitvoert, zorg ervoor dat u aan de volgende voorwaarden voldoet:

- Installeer Google Cloud SDK
- Initialiseer Google Cloud SDK met "gcloud init"
- Installeer Git

#### Uitvoering van het Script

Maak eerst alle scripts uitvoerbaar:
```bash
chmod +x *.sh
chmod +x lib/*.sh
```

Voer het script uit met de volgende parameters:

```bash
./deploy.sh <PROJECT_ID> <REGION> <MIN_REPLICAS> <MAX_REPLICAS>
```
- `<PROJECT_ID>`:  Geef uw Google Cloud Project ID als eerste argument.
- `[REGION]`: Specificeer de regio waarop cloud resources draaien. Indien niet opgegeven, wordt standaard `europe-west2` gebruikt. Dit stelt ook de zone in op de `-b` zone van deze regio (bijvoorbeeld: europe-west2`-b`).
- `[MIN_REPLICAS]`: Specificeer het minimum aantal machines dat draait in de cloud die de applicatie host. Indien niet opgegeven, wordt standaard `2` gebruikt.
- `[MAX_REPLICAS]`: Specificeer het maximum aantal machines dat draait in de cloud die de applicatie host. Indien niet opgegeven, wordt standaard `4` gebruikt.

##### Opties in het deploy script

In de deploy script krijgt u een menu te zien.

U zult deze opties op uw scherm hebben:

- 1. **Start up Phygital from scratch (automatically)**
   Deze optie roept alle functies op van de deploy.sh script, en configureert dus alles van kop tot teen. Hier een lijst van alle onderdelen die worden geconfigureerd: Service keys, Cloud storage, Cloud SQL, Cloud Secret Manager, Cloud Compute Engine, Cloud Domains
- 2. **Upgrade running application to work with new code version**
   Deze optie roept het upgrade.sh script aan, en update de draaiende compute engine instanties om met een nieuwe codeversie te werken. Het script heeft een wachttijd om zero-downtime in de applicatie te prioritiseren.
- 3. **Delete all cloud resources**
   Deze optie verwijdert simpelweg alles uit de cloud omgeving.
- 4. **Back up the database**
   Deze optie roept de databasebackup.sh script aan en vertelt de databank om een kopie van zichzelf te maken, om verlies van gegevens voor te zijn.
- 5. **Developer tools**
   Deze optie breidt de user interface van de script uit naar 11 verschillende opties: Elke individuele gedefinieerde functie in de deploy.sh script kan aangeroepen worden, de jetson nano scripts, en de resterende scripts zoals destroy, upgrade en backup. Natuurlijk ook een optie om te exiten.
- 6. **Exit**
   Deze optie sluit het script af.


#### Aanvullende scripts in de deployment repository voor fysieke installatie

##### Jetson Nano Setup Script (jetsonnanosetup.sh)

Dit script is bedoeld voor het instellen van een apart Jetson Nano bord dat als client in kiosk modus de applicatie zal bezoeken op de website. Het installeert de nodige pakketten, Python libraries, en start de browser in kiosk modus.
Ook runt het automatisch de `sensor.py` script waar er nu uitleg over volgt.

##### Sensor Script (sensor_script.py)

Het sensor script draait ook op het Jetson Nano bord na het uitvoeren van de `jetsonnanosetup.sh` script, en detecteert mensen die met de kiosk bezig zijn. Bij detectie stuurt het API calls naar de dotnet code die wordt uitgevoerd op de Google Cloud Compute Engine.

