# OmniPrice Web API

Una soluzione completa Web API .NET Framework 4.7.2 per servizi di calcolo prezzi, estratta dal sistema PsR (ProductionStepRecorder) con funzionalità avanzate di gestione query.

## 🚀 Funzionalità

- **Calcolo Prezzi Multi-livello** - Sistemi di calcolo prezzi legacy e moderni
- **Sistema QueryManager** - Esecuzione avanzata di query parametriche con gestione timeout
- **Dashboard Interattiva** - Interfaccia di monitoraggio e test in tempo reale
- **Monitoraggio Salute** - Controlli completi di salute API e database
- **Tracciamento Statistiche** - Metriche dettagliate di esecuzione e monitoraggio prestazioni

## 🏗️ Architettura

### Struttura Progetto

```
WebApi.sln
├── WebApi.Misc/                    # Progetto Web API principale
│   ├── Controllers/                # Controller API
│   ├── Default.html               # Dashboard interattiva
│   └── Web.config                 # Configurazione
├── Omnitech.Prezzi.Core/          # Logica business core (da PsR)
├── Omnitech.Prezzi.Infrastructure/ # Implementazioni repository
│   └── QueryManager.cs           # Esecuzione query avanzata
├── Omnitech.DTO/                  # Data Transfer Objects
├── Omnitech.Database/             # Livello accesso dati
├── Omnitech.Enum/                 # Enumerazioni condivise
└── HI.Libs.Utility/               # Librerie utility
```

## 🔧 Avvio Rapido

### Prerequisiti

- .NET Framework 4.7.2 o successivo
- SQL Server 2016+ (supportata edizione Express)
- Visual Studio 2019+ o Visual Studio Code
- IIS Express (incluso con Visual Studio)

### Installazione

1. **Clona il repository**
   ```bash
   git clone <repository-url>
   cd WebApiPrezzi/WebApi
   ```

2. **Ripristina pacchetti NuGet**
   ```bash
   nuget restore WebApi.sln
   ```

3. **Compila la soluzione**
   ```bash
   msbuild WebApi.sln /p:Configuration=Debug
   ```

4. **Configura stringhe di connessione**
   Aggiorna `Web.config` con la tua stringa di connessione SQL Server:
   ```xml
   <appSettings>
     <add key="ConnectionStringPrezzi" value="Data Source=localhost\SQLEXPRESS;Initial Catalog=TuoDB;Integrated Security=True;" />
   </appSettings>
   ```

5. **Avvia l'applicazione**
   - Apri in Visual Studio e premi F5, oppure
   - Usa IIS Express direttamente
   - Accedi alla dashboard su `http://localhost:54340/`

## 📊 Interfaccia Dashboard

La dashboard interattiva (`http://localhost:54340/`) fornisce:

### 🎛️ Funzionalità Principali
- **Monitoraggio Stato in Tempo Reale** - Salute SQL Server e API
- **Sezioni Collassabili** - Interfaccia organizzata per migliore UX
- **Dashboard Statistiche** - Chiamate API, metriche query, tempi risposta
- **Test Endpoint Salute** - Testa tutti gli endpoint API con indicatori stato

### 🔌 Gestione Connessioni Database
- **Switch Rapido** - Cambio veloce tra connessioni Local/Dual
- **Test Connessioni** - Verifica connettività con logging dettagliato
- **Connessioni Personalizzate** - Aggiungi/rimuovi connessioni custom
- **Display Informazioni** - Server, database e stato connessione in tempo reale

### 🔢 Query Manager (Sistema 2-Step)
1. **Step 1: Caricamento Template**
   - Inserisci Query ID (default: 516)
   - Carica template query e identifica parametri ({0}, {1}, etc.)
   - Anteprima struttura query grezza
   - **Bottone Stop** per interrompere caricamento lento

2. **Step 2: Input Parametri & Esecuzione**
   - Generazione dinamica form per parametri richiesti
   - Timer esecuzione in tempo reale
   - Timeout configurabili (180s per Query 516, 90s default)
   - **Bottone Stop** per interrompere query lunghe
   - Display risultati con scorrimento orizzontale

### 🔍 Esecuzione Query SQL Personalizzate
- Esegui query SQL arbitrarie
- Timer esecuzione in tempo reale
- Inserimento query di esempio
- Formattazione risultati con paginazione
- Gestione errori e logging

## 🔌 Endpoint API

### API Calcolo Prezzi

#### v1 - Sistema Legacy
```
POST /api/v1/price/evaluate/{CustomerNo}
Content-Type: application/json

{
  "ConfigItem": {
    // Oggetto configurazione prodotto
  }
}
```

#### v2 - Sistema Moderno (da PsR)
```
GET  /api/v2/price/calculate/{barcode}     # Calcola prezzo per barcode
POST /api/v2/price/calculate               # Calcola con oggetto Order completo
GET  /api/v2/price/details/{barcode}       # Ottieni dettagli ordine senza calcolo
POST /api/v2/price/calculate/batch         # Calcolo batch
```

### API Salute & Gestione
```
GET /api/health/sql                        # Test connessione SQL
GET /api/health/api                        # Test salute API
GET /api/health/statistics                 # Ottieni statistiche esecuzione
GET /api/health/query/{id}/template        # Ottieni template query
POST /api/health/query                     # Esegui query parametrica

# Gestione Connessioni
GET /api/health/connections                # Lista connessioni disponibili
POST /api/health/connections/switch       # Cambia connessione attiva
POST /api/health/connections/test         # Testa connessione specifica
POST /api/health/connections/add          # Aggiungi connessione personalizzata
DELETE /api/health/connections/{name}     # Rimuovi connessione personalizzata
```

## 🗄️ Sistema QueryManager

Sistema avanzato di esecuzione query estratto da PsR con miglioramenti:

### Funzionalità Chiave
- **Sostituzione Parametri Due-Fasi**:
  - Fase 1: Parametri indicizzati (`{0}`, `{1}`) usando `string.Format`
  - Fase 2: Sostituzioni basate su dizionario per sostituzioni avanzate
- **Timeout Configurabili**: 90s default, 180s per Query 516
- **Hint NOLOCK**: Applicati automaticamente alla Query 516 per prestazioni
- **Logica Retry**: 3 tentativi con backoff esponenziale per errori timeout
- **Logging Thread-Safe**: Tutte le operazioni loggate su `C:\WebApiLog\QueryManagerDebug.log`
- **Gestione Connessioni**: Switch dinamico tra connessioni multiple

### Esempio Utilizzo
```csharp
var queryManager = new QueryManager();
queryManager.IdQuery = 516;
queryManager.Args.Add("your-barcode-here");
queryManager.Replace.Add("{CUSTOM_FIELD}", "replacement-value");

if (queryManager.GetQuery())
{
    var results = queryManager.DT; // DataTable con risultati
    var firstRow = queryManager.DR; // Scorciatoia prima riga
    var connection = queryManager.CurrentConnection; // Nome connessione attiva
}
```

### Sistema Connessioni
```csharp
// Cambia connessione
ConnectionManager.SetCurrentConnection("dual");

// Testa connessione
bool isOk = ConnectionManager.TestConnection("local");

// Ottieni informazioni connessione
var info = ConnectionManager.GetConnectionInfo("dual");
```

## 📈 Monitoraggio & Logging

### File di Log
- **Query Manager**: `C:\WebApiLog\QueryManagerDebug.log`
- **Connection Manager**: `C:\WebApiLog\ConnectionManager.log`
- **Price API**: `C:\WebApiLog\Price\LogWebApiPrice.txt`
- **Query Output**: `C:\WebApiLog\query516_our_output_{timestamp}.sql`

### Statistiche Disponibili
- Totale richieste API
- Query database riuscite
- Conteggio timeout query
- Tempi risposta medi
- Tassi successo e rapporti
- Timestamp ultima richiesta
- Connessione attiva e switch history

## 🛠️ Sviluppo

### Compilazione
```bash
# Build debug
msbuild WebApi.sln /p:Configuration=Debug

# Build release
msbuild WebApi.sln /p:Configuration=Release
```

### Testing
Accedi alla dashboard su `http://localhost:54340/` e usa:
- Test endpoint salute
- Query Manager con query di esempio
- Esecuzione SQL personalizzata
- Generazione richieste bulk per statistiche
- Test cambio connessioni e timeout

### Configurazione
Impostazioni chiave in `Web.config`:
```xml
<appSettings>
  <add key="ConnectionStringPrezzi" value="..." />
  <add key="ConnectionStringAvanzamentoProduzione" value="..." />
  <add key="LogModeDebug" value="true" />
  <add key="LogFilename" value="C:\WebApiLog\Price\LogWebApiPrice.txt" />
</appSettings>
```

### Configurazioni Connessioni
Modifica `ConnectionManager.cs` per connessioni predefinite:
```csharp
private static readonly Dictionary<string, string> _connections = new Dictionary<string, string>
{
    { "local", null }, // Caricata da config
    { "dual", "Data Source=192.168.1.90;Initial Catalog=AvanzamentoProduzione;..." },
    { "production", "Data Source=prod-server;..." } // Aggiungi nuove
};
```

## 🤝 Contribuire

1. Forka il repository
2. Crea un branch feature (`git checkout -b feature/funzionalita-fantastica`)
3. Committa le modifiche (`git commit -m 'Aggiungi funzionalità fantastica'`)
4. Pusha al branch (`git push origin feature/funzionalita-fantastica`)
5. Apri una Pull Request

## 📝 Licenza

Questo progetto è licenziato sotto la Licenza MIT - vedi il file [LICENSE](LICENSE) per dettagli.

## 🏢 Informazioni

Sviluppato come parte del sistema di calcolo prezzi Omnitech, estratto e migliorato dall'applicazione PsR (ProductionStepRecorder) per utilizzo API standalone.

---

**Versione Dashboard**: v.4 (con sezioni collassabili e gestione connessioni)
**Versione API**: v.20
**Framework**: .NET Framework 4.7.2