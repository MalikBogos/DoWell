# DoWell - Excel Clone Applicatie

## Projectbeschrijving
DoWell is een vereenvoudigde Microsoft Excel clone applicatie gebouwd met WPF en .NET 8. Het biedt basis spreadsheet functionaliteit inclusief cel bewerking, opmaak, en bestandsbewerkingen.

## Functionaliteiten en Bediening

### Cel Data Bewerking
- **Cel selecteren**: Klik op gewenste cel in het grid
- **Data invoeren**: Typ direct in geselecteerde cel of gebruik de Formula Bar bovenaan
- **Automatische opslag**: Cel informatie wordt automatisch opgeslagen bij cel wijziging (via `DataGrid_CellEditEnding` event)

### Sortering
- **Sorteren op rijnummer**: Klik op de kolom header met rijnummers (linkse kolom) om te sorteren op numerieke volgorde

### Tekst Opmaak
- **Vet**: Gebruik **B** knop in toolbar of **Format > Bold** in menu (Ctrl+B)
- **Cursief**: Gebruik **I** knop in toolbar of **Format > Italic** in menu (Ctrl+I)  
- **Onderstreept**: Gebruik **U** knop in toolbar of **Format > Underline** in menu (Ctrl+U)
- **Geavanceerde opmaak**: Rechtsklik op cel → **"Format Cell..."** voor uitgebreide opmaak dialog

### Kleur Aanpassing
- **Achtergrondkleur**: Gebruik **BG:** dropdown in toolbar voor directe kleur selectie
- **Tekstkleur**: Gebruik **FG:** dropdown in toolbar voor tekstkleur wijziging
- **Kleur preview**: Toolbar dropdowns tonen kleur rechthoekjes met namen voor eenvoudige selectie

### Opmaak Sjablonen
- **Sjabloon toepassen**: Selecteer sjabloon uit **Templates:** dropdown in toolbar
- **Nieuw sjabloon maken**: 
  - Selecteer cel met gewenste opmaak
  - Klik **➕** knop naast Templates dropdown
  - Of gebruik **Format > Create Format Template...** in menu
- **Sjabloon uit context**: Rechtsklik cel → **"Apply Template"**

### Rijen en Kolommen
- **Rij toevoegen**: **➕ Row** knop in toolbar
- **Rij verwijderen**: **➖ Row** knop in toolbar (met bevestiging dialog)
- **Kolom toevoegen**: **➕ Column** knop in toolbar  
- **Kolom verwijderen**: **➖ Column** knop in toolbar (met bevestiging dialog)

### Bestandsbeheer
- **Nieuw bestand**: **File > New Workbook** in menu (Ctrl+N)
- **Openen**: **📁 Open** knop in toolbar of **File > Open...** (Ctrl+O)
- **Opslaan**: **💾 Save** knop in toolbar of **File > Save** (Ctrl+S)
- **Opslaan als**: **File > Save As...** in menu (Ctrl+Shift+S)
- **Ondersteunde formaten**: .dwl (DoWell native), .json

### Zoek Functionaliteit  
- **Zoeken**: **Edit > Find...** in menu of sneltoets (Ctrl+F)
- **Zoek opties**: Match case checkbox voor hoofdlettergevoelig zoeken
- **Zoek navigatie**: "Find Next" knop om door zoekresultaten te bladeren

### Context Menu (Rechtsklik)
- **Cut/Copy/Paste**: Standaard klembord operaties (Ctrl+X/C/V)
- **Format Cell...**: Opent geavanceerde opmaak dialog
- **Clear Contents**: Wist cel inhoud
- **Apply Template**: Toepassen van geselecteerd sjabloon

### Sneltoetsen
- **Ctrl+N**: Nieuwe werkmap
- **Ctrl+O**: Werkmap openen  
- **Ctrl+S**: Werkmap opslaan
- **Ctrl+Shift+S**: Opslaan als
- **Ctrl+B**: Vet maken/ongedaan maken
- **Ctrl+I**: Cursief maken/ongedaan maken
- **Ctrl+U**: Onderstrepen/ongedaan maken
- **Ctrl+F**: Zoek dialog openen
- **Ctrl+X/C/V**: Knippen/Kopiëren/Plakken

### Status Informatie
- **Status bar**: Toont werkmap naam, aantal rijen/kolommen, en operatie feedback
- **Real-time feedback**: Groene/rode status berichten voor geslaagde/mislukte operaties

## Technologie Stack

### Framework & Runtime
- **.NET 8.0** - Target framework
- **WPF (Windows Presentation Foundation)** - UI framework voor Windows desktop applicaties

### Libraries & Packages

#### NuGet Packages
De volgende NuGet packages worden gebruikt in dit project (zoals gedefinieerd in `DoWell.csproj`):

1. **CommunityToolkit.Mvvm (v8.4.0)**
   - Doel: MVVM patroon implementatie met source generators
   - Gebruikt voor: ObservableProperty, RelayCommand attributen

2. **Microsoft.EntityFrameworkCore (v9.0.8)**
   - Doel: Object-Relational Mapping (ORM) framework
   - Gebruikt voor: Database toegang en data persistentie

3. **Microsoft.EntityFrameworkCore.Design (v9.0.8)**
   - Doel: Design-time ondersteuning voor Entity Framework
   - Gebruikt voor: Migrations en scaffolding tools

4. **Microsoft.EntityFrameworkCore.Proxies (v9.0.8)**
   - Doel: Lazy loading ondersteuning
   - Gebruikt voor: Automatisch laden van gerelateerde entiteiten

5. **Microsoft.EntityFrameworkCore.SqlServer (v9.0.8)**
   - Doel: SQL Server database provider
   - Gebruikt voor: Verbinding met SQL Server LocalDB database

6. **Microsoft.EntityFrameworkCore.Tools (v9.0.8)**
   - Doel: Package Manager Console tools
   - Gebruikt voor: Database migrations via Package Manager Console

7. **SQLite (v3.13.0)**
   - Doel: Lightweight database engine (opgenomen maar niet actief gebruikt)
   - Status: Beschikbaar voor toekomstige implementatie

### Database
- **SQL Server LocalDB** - Lokale database voor development
- **Database naam**: DoWellDB
- **Connection string**: Gebruikt Integrated Security met LocalDB instance

## Project Structuur

### Chatlogs AI
- Chatlogs van Claude AI assistentie voor probleemoplossing en feature integratie

### Models
- `Cell.cs` - Entiteit voor spreadsheet cellen
- `Workbook.cs` - Entiteit voor werkmap
- `FormatTemplate.cs` - Entiteit voor opmaak sjablonen

### ViewModels
- `MainViewModel.cs` - Hoofd view model voor applicatie logica
- `CellViewModel.cs` - View model voor individuele cellen
- `ViewModelBase.cs` - Basis klasse voor view models

### Views
- `MainWindow.xaml/.cs` - Hoofd applicatie venster
- `FormatCellDialog.xaml/.cs` - Dialog voor cel opmaak
- `InputDialog.xaml/.cs` - Algemene input dialog
- `FindDialog.xaml/.cs` - Zoek functionaliteit dialog

### Data
- `DoWellContext.cs` - Entity Framework DbContext
- Migration bestanden voor database schema

### Converters
- `BoolToFontWeightConverter` - Converteert boolean naar FontWeight
- `BoolToFontStyleConverter` - Converteert boolean naar FontStyle
- `BoolToTextDecorationConverter` - Converteert boolean naar TextDecoration
- `RowNumberConverter` - Converteert rij index naar display nummer
- `SimpleRowNumberConverter` - Vereenvoudigde rij nummer converter

## Gebruikte Ontwikkeltools
- **Visual Studio 2022** (versie 17.11.35431.28 of hoger)
- **SQL Server LocalDB** - Voor database development
- **Entity Framework Core Tools** - Voor database migrations

## Externe Bronnen & Referenties

### Microsoft Documentatie
- [WPF Documentation](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [CommunityToolkit.Mvvm Documentation](https://docs.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/)

### Design Patterns
- **MVVM (Model-View-ViewModel)** patroon voor UI architectuur
- **Repository Pattern** via Entity Framework DbContext
- **Command Pattern** via RelayCommand implementatie

## Database Schema
De applicatie gebruikt de volgende database tabellen:
- **Workbooks** - Opslag van werkmap informatie
- **Cells** - Opslag van cel data en opmaak
- **FormatTemplates** - Opslag van herbruikbare opmaak sjablonen

## Bestandsformaten
- **.dwl** - DoWell native formaat (JSON gebaseerd)
- **.json** - Standaard JSON export/import

## Systeemvereisten
- Windows 10/11
- .NET 8.0 Runtime
- SQL Server LocalDB (geïnstalleerd met Visual Studio)
- Minimaal 100 MB vrije schijfruimte

## Installatie & Setup
1. Clone of download het project
2. Open `DoWell.sln` in Visual Studio 2022
3. Herstel NuGet packages (`dotnet restore`)
4. Run database migrations (automatisch bij eerste start)
5. Build en run de applicatie

## Belangrijkste Klassen en Functies

### Core Models (Data Entiteiten)

#### `Cell` Klasse
**Doel**: Representeert een individuele spreadsheet cel in de database
**Belangrijkste eigenschappen**:
- `Row/Column`: Positie van de cel in het grid
- `Value`: Tekstuele inhoud van de cel
- `IsBold/IsItalic/IsUnderline`: Boolean waarden voor tekst opmaak
- `BackgroundColor/ForegroundColor`: Hex kleuren voor cel styling
- `FormatTemplateId`: Verwijzing naar toegepaste opmaak sjabloon
- `WorkbookId`: Foreign key naar parent werkmap

#### `Workbook` Klasse
**Doel**: Representeert een complete spreadsheet werkmap
**Belangrijkste eigenschappen**:
- `Name`: Gebruiksvriendelijke naam van de werkmap
- `Author`: Maker van de werkmap
- `FilePath`: Locatie waar werkmap is opgeslagen
- `Cells`: Collectie van alle cellen in de werkmap
- `FormatTemplates`: Collectie van beschikbare opmaak sjablonen

#### `FormatTemplate` Klasse
**Doel**: Herbruikbare opmaak configuraties voor cellen
**Belangrijkste eigenschappen**:
- `Name`: Gebruiksvriendelijke naam van het sjabloon
- `FontFamily/FontSize`: Lettertype configuratie
- `IsBold/IsItalic/IsUnderline`: Tekst styling opties
- `BackgroundColor/ForegroundColor`: Kleur configuratie

### ViewModels (Presentation Layer)

#### `MainViewModel` Klasse
**Doel**: Centrale applicatie logica en state management
**Belangrijkste functies**:

- `LoadWorkbookData()`: Laadt werkmap data uit database naar UI grid
- `SaveChangesToDatabase()`: Persisteert UI wijzigingen naar database
- `AddRow()/RemoveRow()`: Dynamisch toevoegen/verwijderen van rijen
- `AddColumn()/RemoveColumn()`: Dynamisch toevoegen/verwijderen van kolommen
- `SaveWorkbookToFile()/LoadWorkbookFromFile()`: JSON serialisatie voor bestand I/O
- `CreateFormatTemplate()`: Maakt nieuwe opmaak sjabloon van geselecteerde cel

**Belangrijkste eigenschappen**:
- `GridData`: 2D collectie van CellViewModels voor UI binding
- `SelectedCell`: Momenteel geselecteerde cel voor formatting
- `CurrentWorkbook`: Actieve werkmap entiteit
- `FormatTemplates`: Beschikbare opmaak sjablonen

#### `CellViewModel` Klasse
**Doel**: UI wrapper voor Cell entiteit met WPF binding ondersteuning
**Belangrijkste functies**:
- `UpdateCell()`: Synchroniseert ViewModel wijzigingen naar Model
- `ToggleBold()/ToggleItalic()/ToggleUnderline()`: Tekst opmaak commando's
- `SetBackgroundColor()/SetForegroundColor()`: Kleur wijziging commando's
- `ApplyTemplate()`: Past FormatTemplate toe op cel
- `GetBrushFromHex()`: Converteert hex kleuren naar WPF Brush objecten

### Data Access Layer

#### `DoWellContext` Klasse (Entity Framework DbContext)
**Doel**: Database toegang en entiteit configuratie
**Belangrijkste functies**:
- `OnConfiguring()`: Configureert SQL Server LocalDB connectie
- `OnModelCreating()`: Definieert entiteit relaties en constraints
- Seed data voor initial werkmap en format templates

**Database Relaties**:
- Workbook 1:N Cells (Cascade delete)
- Workbook 1:N FormatTemplates (Cascade delete)
- FormatTemplate 1:N Cells (No action delete - voorkomt cyclische deletes)
- Unique constraint op (WorkbookId, Row, Column) voor cel positie

### UI Components (Views)

#### `MainWindow` Klasse
**Doel**: Hoofd applicatie interface met spreadsheet grid
**Belangrijkste functies**:
- `UpdateDataGridColumns()`: Dynamisch genereren van DataGrid kolommen
- `CreateCellTemplate()`: WPF DataTemplate voor cel weergave met styling
- `CreateCellEditingTemplate()`: WPF DataTemplate voor cel bewerking
- `DataGrid_SelectedCellsChanged()`: Synchroniseert cel selectie met ViewModel
- Event handlers voor toolbar acties (Bold, Italic, Underline, kleuren)

#### Dialog Klassen
**`FormatCellDialog`**: Geavanceerde cel opmaak interface
- Tab-based interface voor font styling en cel waarde
- Real-time preview van opmaak wijzigingen
- Kleur picker controls met visuele feedback

**`InputDialog`**: Herbruikbare tekst input voor sjabloon namen
**`FindDialog`**: Zoek functionaliteit door spreadsheet cellen

### Utility Components

#### Value Converters
**`BoolToFontWeightConverter`**: Converteert `bool IsBold` naar `FontWeight.Bold/Normal`
**`BoolToFontStyleConverter`**: Converteert `bool IsItalic` naar `FontStyle.Italic/Normal`  
**`BoolToTextDecorationConverter`**: Converteert `bool IsUnderline` naar TextDecoration
**`RowNumberConverter`**: Converteert rij index naar Excel-stijl rij nummers (1, 2, 3...)

## Data Flow Architecture

1. **User Input** → UI Events in MainWindow
2. **UI Events** → RelayCommands in MainViewModel  
3. **Commands** → Business logic methods in ViewModels
4. **ViewModels** → Entity updates via CellViewModel.UpdateCell()
5. **Entities** → Database persistence via DoWellContext.SaveChanges()
6. **Database** → UI updates via ObservableProperty change notifications

## Key Design Decisions

### MVVM Pattern Implementation
- **Models**: Pure data entities zonder UI logica
- **ViewModels**: UI state en commands, Entity Framework interactie
- **Views**: Alleen XAML binding en event forwarding

### Entity Framework Strategy  
- **Lazy Loading**: Automatisch laden van gerelateerde entiteiten
- **Change Tracking**: EF Core detecteert wijzigingen voor optimale database updates
- **Migrations**: Database schema versioning en deployment

### Performance Optimizations
- ObservableCollection voor efficiënte UI updates
- Conditional database saves (alleen bij daadwerkelijke cel wijzigingen)
- Template-based DataGrid voor geheugen efficiëntie bij grote grids

## Opmerkingen
- Het project gebruikt Entity Framework als absolute vereiste voor financiering
- Alle functionaliteit is beperkt gehouden tot de gespecificeerde requirements
- CommunityToolkit.Mvvm wordt gebruikt om code zo compact mogelijk te houden
- Database migrations worden automatisch uitgevoerd bij opstarten
- Functionaliteit voor filteren, wijzigen van lettertypen en basisformules zijn nog niet beschikbaar
- De README.md werd deels gegenereerd door Claude AI

AI werd gebruikt voor probleemoplossing, bugfixes, algemene hulp bij het programmeren en integratie van functies in de code

Adegeo. (z.d.). DataGrid - WPF. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/datagrid -> algemene informatie

chatlog1 - Claude. (z.d.-b). https://claude.ai/chat/c35adc06-7195-4c50-aeea-5677317a12ef

chatlog2 - Claude. (z.d.-a). https://claude.ai/chat/2e529b24-212c-4800-b5e8-bc5f2bda29d6

chatlog3 - Claude. (z.d.-c). https://claude.ai/chat/c92e441f-e57e-446d-b172-cf42d1374a20

chatlog4 - Claude. (z.d.-d). https://claude.ai/chat/eb0abd0c-6f62-4c47-bcad-72e41541f18b

chatlog5 - Claude. (z.d.-e). https://claude.ai/chat/7bf6ccb6-c090-4eba-98c9-7b35f2e0cf62

chatlog6 - Claude. (z.d.-f). https://claude.ai/chat/c93243f4-5ebc-487a-add4-d26f93882ce3

chatlog7 - Claude. (z.d.-g). https://claude.ai/chat/0fc228eb-e227-4118-b7bf-1c6c0f507cda

chatlog8 - Claude. (z.d.-h). https://claude.ai/chat/21b55275-03ee-4b61-9149-7b5abff49fb2

chatlog9 - Claude. (z.d.-i). https://claude.ai/chat/bf895751-80e4-4e1a-9ddc-98532da1acf6

chatlog10 - Claude. (z.d.-j). https://claude.ai/chat/d36fbefe-7b7d-4147-8393-771305d3e03a

chatlog11 - Claude. (z.d.-k). https://claude.ai/chat/813a61cc-333f-4b58-b9a3-2fe12c7baa17

chatlog12 - Claude. (z.d.-l). https://claude.ai/chat/acce5b74-4725-46fb-8bc7-10d01eceb2e8

chatlog13 - Claude. (z.d.-m). https://claude.ai/chat/0dc91c10-b2f8-46ae-90db-eedf6a792a0a

chatlog14 - Claude. (z.d.-n). https://claude.ai/chat/6717c36e-be07-473b-b151-9b74b9d3a6e5

chatlog15 - Claude. (z.d.-o). https://claude.ai/chat/47ba045a-6684-4aca-9ac7-d52fc81ad23d

MESCIUS inc. (2021, 30 november). How to Use FlexGrid, a WPF Datagrid Control, in Your Desktop Application [Video]. YouTube. https://www.youtube.com/watch?v=fguYA_qzmYw -> algemene informatie

CodingHacks. (2020, 28 april). Entity Framework | What is Entity Framework | Entity Framework in MVC [Video]. YouTube. https://www.youtube.com/watch?v=FZTqnBEaxuc -> algemene informatie

tutorialsEU - C#. (2023, 26 januari). Get started with ENTITY FRAMEWORK in C#! [Video]. YouTube. https://www.youtube.com/watch?v=gQ2yBG3-jKI -> algemene informatie

Kampa Plays. (2022, 3 november). C# WPF Tutorial #1 - What is WPF? [Video]. YouTube. https://www.youtube.com/watch?v=t9ivUosw_iI -> algemene informatie, leren werken met WPF

Kevin Bost. (2023, 15 oktober). C#/WPF - Learning the DataGrid [Video]. YouTube. https://www.youtube.com/watch?v=wg1HUSIowTA -> algemene informatie

Adegeo. (z.d.). DataGrid - WPF. Microsoft Learn. https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/datagrid -> DataGrid informatie