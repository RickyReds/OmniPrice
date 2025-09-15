# Registro Modifiche

Tutte le modifiche notevoli al progetto OmniPrice Web API saranno documentate in questo file.

Il formato è basato su [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
e questo progetto aderisce al [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [In sviluppo]

## [4.0.0] - 2025-01-15

### Aggiunto
- **Sistema Gestione Connessioni Database** - Gestione completa connessioni multiple
  - ConnectionManager con connessioni predefinite 'local' e 'dual'
  - Switch rapido tra connessioni con bottoni dedicati
  - Test connessioni con logging dettagliato e timing
  - Aggiunta/rimozione connessioni personalizzate
  - Display real-time informazioni server/database

- **Bottoni Stop per Query Manager** - Controllo completo esecuzione query
  - Step 1: Bottone stop per caricamento template (180px fisso)
  - Step 2: Bottone stop per esecuzione query (140px fisso)
  - AbortController per interruzione real-time di entrambe le operazioni
  - Reset automatico bottoni in tutti i casi (successo/errore/abort)

- **Sezioni Dashboard Collassabili** - Tutte le sezioni dashboard possono essere espanse/collassate per migliore organizzazione
  - Animazioni CSS fluide con transizioni opacità e altezza
  - Icone toggle (▼/▶) con feedback visivo
  - Stati sezione persistenti durante la sessione
  - Dashboard versione aggiornata a v.4

### Modificato
- **Miglioramento Scorrimento Orizzontale** - Comportamento scorrimento tabelle migliorato
  - Barre scorrimento orizzontali ora applicate solo alle tabelle risultati, non alle aree testo query
  - Aree testo query usano word wrapping per migliore leggibilità
  - Mantenuta larghezza minima tabelle per display colonne appropriato

- **Bottoni Dimensione Fissa** - Prevenzione resize fastidioso
  - Bottoni Query Manager con larghezza fissa (140px-180px)
  - Layout consistente senza cambiamenti dimensioni durante operazioni

- **Display Nome Connessione** - Informazioni connessione nei risultati
  - Nome connessione attiva mostrato nei risultati query con styling blu
  - Integrazione con sistema ConnectionManager

### Corretto
- **Display Testo Query** - Risolti problemi scorrimento orizzontale nelle aree anteprima query
  - Applicato `white-space: pre-wrap` e `word-wrap: break-word` agli elementi testo query
  - Prevenute barre scorrimento orizzontali su aree testo mantenendo funzionalità scorrimento tabelle

- **Allineamento Bottoni Test** - Bottoni test connessioni sempre allineati a destra
  - Layout grid migliorato per allineamento consistente
  - Placeholder invisibili per connessioni attive senza bottone "Usa"

## [3.0.0] - 2025-01-14

### Aggiunto
- **Timer Esecuzione Real-Time** - Tracciamento tempo esecuzione live per tutte le operazioni query
  - Aggiornamenti timer visivi ogni 100ms durante esecuzione query
  - Display tempo esecuzione lato server dopo completamento
  - Formato timer consistente tra sezioni Query Manager e SQL Personalizzate

- **Sistema Query Manager Avanzato** - Processo esecuzione query in due step
  - **Step 1**: Carica template query e identifica parametri richiesti ({0}, {1}, etc.)
  - **Step 2**: Generazione dinamica form parametri ed esecuzione query
  - Rilevamento parametri real-time e creazione form
  - Funzionalità anteprima query con evidenziazione sintassi

### Modificato
- **Gestione Parametri QueryManager** - Logica preservazione parametri migliorata
  - Collezione Args non più svuotata quando barcode è null
  - Mantiene stato parametri tra caricamento template ed esecuzione
  - Migliore gestione errori per mismatch parametri

### Corretto
- **Errori Indice Parametri Query** - Risolti errori "L'indice deve essere maggiore o uguale a zero"
  - Corretta pulizia collezione Args quando si usano parametri personalizzati
  - Gestione appropriata stato parametri nel processo esecuzione due-step

## [2.0.0] - 2025-01-13

### Aggiunto
- **Infrastruttura QueryManager** - Sistema gestione query completo estratto da PsR
  - Sistema sostituzione parametri due-fasi (indicizzati + basati su dizionario)
  - Timeout configurabili con gestione speciale per Query 516 (180 secondi)
  - Hint NOLOCK automaticamente applicati alla Query 516 per ottimizzazione prestazioni
  - Logica retry con backoff esponenziale per scenari timeout
  - Logging thread-safe su `C:\WebApiLog\QueryManagerDebug.log`

- **Endpoint API Gestione Salute**
  - `GET /api/health/sql` - Test connessione SQL Server
  - `GET /api/health/api` - Verifica salute Web API
  - `GET /api/health/statistics` - Statistiche esecuzione e metriche
  - `GET /api/health/query/{id}/template` - Recupero template query
  - `POST /api/health/query` - Esecuzione query parametriche

- **Interfaccia Dashboard Interattiva** - Interfaccia monitoraggio e test completa
  - Monitoraggio stato real-time per SQL Server e Web API
  - Tracciamento statistiche con chiamate API, metriche query e tempi risposta
  - Test endpoint salute con indicatori stato
  - Strumenti generazione richieste per popolamento statistiche

### Modificato
- **Architettura Progetto** - Struttura multi-tier migliorata
  - Aggiunto progetto `Omnitech.Prezzi.Infrastructure` con QueryManager
  - Migliorata separazione responsabilità tra livelli
  - Logging e gestione errori migliorati ovunque

## [1.0.0] - Release Iniziale

### Aggiunto
- **API Calcolo Prezzi Core** - Sistema calcolo prezzi multi-versione
  - Sistema legacy v1 con valutazione basata su ConfigItem
  - Sistema moderno v2 estratto da PsR con calcoli basati su Order
  - Supporto per calcoli prezzi basati su barcode
  - Capacità elaborazione batch

- **Architettura Multi-Tier** - Struttura progetto completa
  - `WebApi.Misc` - Progetto Web API principale con controller
  - `Omnitech.Prezzi.Core` - Logica business core
  - `Omnitech.DTO` - Data Transfer Objects
  - `Omnitech.Database` - Livello accesso dati con Dapper
  - `Omnitech.Enum` - Enumerazioni condivise

- **Dashboard Base** - Interfaccia monitoraggio iniziale
  - Monitoraggio stato SQL Server e API
  - Display statistiche base
  - Gestione configurazione

### Dettagli Tecnici

#### Miglioramenti Database
- **Gestione Stringhe Connessione** - Gestione configurazione migliorata
- **Ottimizzazione Query** - Hint NOLOCK per query critiche per prestazioni
- **Operazioni Thread-Safe** - Protezione accesso concorrente per logging ed esecuzione query

#### Miglioramenti API
- **Gestione Errori** - Messaggi errore completi e logging
- **Monitoraggio Prestazioni** - Tracciamento tempo esecuzione e statistiche
- **Controlli Salute** - Verifica salute sistema automatizzata

#### Funzionalità Dashboard
- **Design Responsive** - Interfaccia mobile-friendly con animazioni fluide
- **Aggiornamenti Real-Time** - Indicatori stato live e timer esecuzione
- **Test Interattivi** - Strumenti integrati per test API e query
- **Visualizzazione Statistiche** - Display metriche completo

---

### Note Sviluppo

**Framework**: .NET Framework 4.7.2
**Database**: SQL Server 2016+ con Dapper ORM
**Frontend**: HTML5 + Vanilla JavaScript con CSS Grid/Flexbox
**Deployment**: IIS Express per sviluppo, configurabile per produzione

### Note Migrazione

Quando si aggiorna da versioni precedenti:
1. Aggiorna stringhe connessione in `Web.config`
2. Assicurati che la directory `C:\WebApiLog\` esista per logging
3. Testa funzionalità QueryManager con query esistenti
4. Verifica accessibilità dashboard su `http://localhost:54340/`
5. Configura connessioni multiple nel ConnectionManager se necessario