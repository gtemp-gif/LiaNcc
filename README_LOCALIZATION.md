# Gestione della Localizzazione - Progetto LiaNcc

Questo documento descrive in dettaglio come è strutturata e come funziona la localizzazione (traduzione multilingua) all'interno del progetto LiaNcc, coprendo Database, WebAPI e FrontEnd.

## 1. Architettura del Database

Il sistema di localizzazione si basa su due tabelle principali presenti nel database:

### Tabella `Languages`
Contiene le lingue supportate dal sistema.
- **Id**: Identificativo univoco (GUID).
- **Code**: Codice ISO della lingua (es. `it`, `en`, `fr`).
- **Name**: Nome descrittivo (es. `Italiano`, `English`).
- **IsDefault**: Indica se è la lingua predefinita del sistema (generalmente `it`).
- **IsActive**: Se la lingua è attualmente abilitata.

### Tabella `LocalizedContents`
Implementa un pattern flessibile per tradurre qualsiasi campo di qualsiasi entità senza dover modificare lo schema delle tabelle principali.
- **Id**: Identificativo univoco (GUID).
- **EntityName**: Il nome dell'entità a cui si riferisce la traduzione (es. `Tour`, `PageSection`, `Service`).
- **EntityId**: Il GUID del record specifico (es. l'Id del tour "Matera").
- **ContentKey**: Il nome del campo che stiamo traducendo (es. `Name`, `Description`, `HeroTitle`).
- **LanguageCode**: Il codice della lingua della traduzione (es. `en`).
- **ContentValue**: Il testo tradotto.

## 2. Come Funziona il Meccanismo

L'approccio prevede che le tabelle principali (es. `Tours`, `PageSections`) contengano i testi nella **lingua di default** (Italiano).
Quando un utente naviga il sito in una lingua diversa (es. Inglese), il sistema interroga la tabella `LocalizedContents` per cercare eventuali sovrascritture. Se la traduzione esiste, viene mostrata al posto del testo originale; se non esiste, si usa il testo di fallback in italiano.

## 3. Esempi Pratici di Utilizzo

### Esempio A: Tradurre un Tour

Supponiamo di avere un Tour nella tabella `Tours`:
- **Id**: `11111111-1111-1111-1111-111111111111`
- **Name**: `Matera - La città dei Sassi`
- **Description**: `Un viaggio indimenticabile...`

Per fornire la traduzione in inglese, nel BackOffice (e quindi nella tabella `LocalizedContents`) avremo:
- Record 1: `EntityName` = `Tour`, `EntityId` = `1111...`, `ContentKey` = `Name`, `LanguageCode` = `en`, `ContentValue` = `Matera - The City of Stones`
- Record 2: `EntityName` = `Tour`, `EntityId` = `1111...`, `ContentKey` = `Description`, `LanguageCode` = `en`, `ContentValue` = `An unforgettable journey...`

### Esempio B: Recupero dati lato WebAPI

Quando la WebAPI deve restituire i dati al FrontEnd, può farlo in due modi:

**Metodo 1: Restituire tutto (Gestione lato FE)**
L'endpoint `/api/tours` restituisce il Tour con una proprietà navigazionale `LocalizedContents` inclusa. Sarà il FrontEnd a filtrare per la lingua corrente.

**Metodo 2: Risoluzione lato API (Consigliato)**
L'endpoint accetta un parametro `?culture=en`. La query LINQ legge il record base e applica la traduzione in tempo reale se esiste.

*Esempio di query LINQ nel Repository:*
```csharp
public async Task<TourResponse> GetTourByIdAsync(Guid id, string culture)
{
    var tour = await _context.Tours
        .Include(t => t.LocalizedContents)
        .FirstOrDefaultAsync(t => t.Id == id);

    if (tour == null) return null;

    // Funzione helper per risolvere il testo
    string GetTranslation(string key, string fallback)
    {
        var translation = tour.LocalizedContents
            .FirstOrDefault(l => l.LanguageCode == culture && l.ContentKey == key);
        return translation != null ? translation.ContentValue : fallback;
    }

    return new TourResponse
    {
        Id = tour.Id,
        Name = GetTranslation("Name", tour.Name),
        Description = GetTranslation("Description", tour.Description)
    };
}
```

### Esempio C: Lettura Contenuti delle Pagine (CMS) nel FrontEnd

Nel progetto `LiaNcc.FE`, la lingua selezionata dall'utente è generalmente gestita tramite un cookie o la QueryString (es. `?culture=en`), che imposta il `Thread.CurrentThread.CurrentUICulture`.

Se stiamo renderizzando la sezione "Chi Siamo" della Homepage:

1. Chiamiamo l'API client: `await _cmsApiClient.GetSitePageBySlugAsync("home", currentCulture);`
2. L'API ci restituisce la pagina con i testi già localizzati per la lingua richiesta (oppure con la lista delle traduzioni).
3. Nel Razor View `Index.cshtml`:

```html
@model LiaNcc.Models.DTOs.Responses.SitePageResponse
@{
    // Supponendo che il viewModel abbia già risolto le traduzioni
    var aboutSection = Model.Sections.FirstOrDefault(s => s.BaseName == "Chi Siamo");
}

<section class="py-32 bg-black">
    <div class="max-w-7xl mx-auto text-center">
        <!-- Mostra il titolo tradotto ("About Us" se in inglese, "Chi Siamo" se in italiano) -->
        <h2 class="text-white">@aboutSection.Title</h2>

        <!-- Mostra la descrizione tradotta -->
        <p class="text-gray-400">@aboutSection.Description</p>
    </div>
</section>
```

## 4. Gestione nel BackOffice (BO)

Attualmente nel BO (sotto il menu "Localizzazione") c'è l'interfaccia per gestire l'abilitazione delle lingue base (es. `it`, `en`).

Per espandere la gestione dei contenuti, si segue questo pattern:
1. Quando si modifica un'entità (es. un Servizio), si prevede una tab "Traduzioni".
2. La tab mostra i campi traducibili (Name, Description).
3. Per ogni lingua supportata diversa dal default, si presenta un input field per salvare il valore.
4. Al salvataggio, il controller API fa un "Upsert" (Update o Insert) sulla tabella `LocalizedContents` usando l'`EntityId` del servizio modificato, la `LanguageCode` della tab corrente, e la `ContentKey` corrispondente al campo.
