# 🛒 Enterprise E-Commerce System

Architettura Event-Driven a Microservizi sviluppata su piattaforma **.NET 10, progettata per scalabilità, disaccoppiamento dei domini e 
resilienza operativa. Il progetto adotta il pattern **Database-per-Service**, utilizza **Apache Kafka** per la messaggistica asincrona e 
YARP come API Gateway.

---

## 🏛 Architettura del Sistema

Il sistema orchestra diversi microservizi isolati, un Gateway unificato e un Frontend containerizzato.
                   ┌─────────────────────────┐
                   │    Ecommerce.Frontend   │
                   └────────────┬────────────┘
                                │
                                ▼
                   ┌─────────────────────────┐
                   │    Ecommerce.Gateway    │ (YARP Reverse Proxy)
                   └────────────┬────────────┘
                                │
     ┌──────────────────────────┼──────────────────────────┐
     ▼                          ▼                          ▼
┌──────────────┐         ┌──────────────┐         ┌────────────────────┐
│ Altri Servizi│         │  Magazzino   │         │ Approvvigionamento │
│ (Utenti, ecc)│         │   WebApi     │         │       WebApi       │
└──────┬───────┘         └──────┬───────┘         └─────────┬──────────┘
       │                        │                           │
       ▼                        ▼                           ▼
[ Database SQL ]         [ Magazzino DB ]           [ Approvvigionamento DB ]
                                │                           ▲
                                └────────► [ Kafka ] ───────┘
                                     (Event Bus Asincrono)

---

## 📂 Struttura del Progetto e Componenti

La repository è strutturata come monorepo. Di seguito la documentazione estensiva di ogni modulo presente nel progetto.

### 1. Root del Progetto

I file principali di configurazione e orchestrazione alla radice del repository:

- `Ecommerce.slnx`: File di soluzione principale per .NET 10.
- `Ecommerce.sln.DotSettings.user`: Configurazioni specifiche dell'IDE (es. JetBrains Rider).
- `docker-compose.yml`: File di orchestrazione Docker che definisce e avvia i container per l'API Gateway, i microservizi,
   Apache Kafka (broker ed ecosistema), il Frontend e i database.
- `nuget.config` e cartella `nuget-local/`: Configurazioni per la gestione dei pacchetti NuGet, inclusa l'abilitazione di feed locali o custom.
- `README.md`: Il presente file di documentazione.
  
### 2. Strato Dati (Cartella `database/`)

Implementazione del pattern Database-per-Service. Ogni dominio ha il suo script di inizializzazione isolato.

- `init-databases.sh`: Script bash che viene eseguito all'avvio del container del database per lanciare tutti gli script SQL.
- `approvvigionamento.sql`: Schema e dati iniziali per il dominio Approvvigionamento.
- `magazzino.sql`: Schema e dati iniziali per il dominio Magazzino.
- `ordini.sql`: Schema e dati iniziali per il dominio Ordini.
- `pagamenti.sql`: Schema e dati iniziali per il dominio Pagamenti.
- `utenti.sql`: Schema e dati iniziali per il dominio Utenti.

### 3. Componenti Condivisi (Cartella `src/BuildingBlocks/`)

Librerie cross-cutting utilizzate da tutti i microservizi per garantire uniformità tecnica e di dominio.

- `Common.Auth`: Modulo centralizzato per l'autenticazione.
- `ClaimTypesCustom.cs`: Definizione di costanti per i claim JWT personalizzati.
- `ITokenGenerator.cs` / `JwtTokenGenerator.cs`: Astrazione e implementazione per la generazione dei JSON Web Token.
- `JwtSettings.cs`: Modello per il binding delle configurazioni JWT da `appsettings.json`.
- `ServiceCollectionExtensions.cs`: Metodi di estensione per registrare facilmente i servizi di autenticazione nella Dependency Injection
   dei vari microservizi.


- `Domain.Common`: Astrazioni di base del Domain-Driven Design.
- `Entity.cs`: Classe base astratta per tutte le entità di dominio (gestione ID, uguaglianza, eventi di dominio).


- `EventBus.Contracts`: Libreria dei contratti per la messaggistica asincrona.
- `IntegrationEvent.cs`: Interfaccia o classe base per tutti gli eventi pubblicati sul message broker.
- `Events/OrderCreatedEvent.cs`: Contratto specifico dell'evento scatenato alla creazione di un nuovo ordine.



### 4. Microservizi di Dominio (Cartella `src/Services/`)

#### 4.1 Servizio Approvvigionamento (`Approvvigionamento/`)

Gestisce la fornitura dei prodotti e il riassortimento.

- `Approvvigionamento.WebApi`: Punto di ingresso HTTP e host del servizio. Contiene il `Program.cs`, il `Dockerfile` per la containerizzazione, e i controller REST (`Controllers/ForniturePasController.cs`). Include anche i worker in background (`Consumers/ScortaBassaConsumer.cs`) che ascoltano Kafka.
- `Approvvigionamento.Business`: Contiene la logica applicativa principale, orchestrando le operazioni di fornitura tramite `Services/FornituraService.cs`.
- `Approvvigionamento.Repository`: Strato di accesso ai dati basato su Entity Framework Core. Contiene il contesto del database (`ApprovvigionamentoDbContext.cs`) e i modelli fisici delle tabelle (`Entities/Fornitura.cs`, `Entities/RichiestaRifornimento.cs`).
* **`Approvvigionamento.Shared`**: Libreria contrattuale interna. Contiene gli oggetti di trasferimento dati (cartella `DTOs`) e gli eventi generati o utilizzati da questo dominio (`Events/RiassortimentoCompletatoEvent.cs`, `Events/ScortaBassaEvent.cs`).
* **`Approvvigionamento.ClientHttp`**: Libreria per consentire ad altri microservizi di chiamare sincronicamente l'Approvvigionamento tramite chiamate REST tipizzate.

#### 4.2 Servizio Magazzino (`Magazzino/`)

Gestisce le giacenze, le scorte e la logistica dei prodotti.

* **`Magazzino.WebApi`**: Entry point del microservizio (`Program.cs`) che configura i servizi, la pipeline HTTP e inietta le librerie di Kafka (`Confluent.Kafka`, `Utility.Kafka.2025`).
* **`Magazzino.Business`**: Nucleo decisionale. Include l'integrazione nativa con il broker di eventi:
* `Kafka/MagazzinoKafkaTopics.cs`: Definizione delle costanti per i topic di iscrizione/pubblicazione.
* `Kafka/MagazzinoMessageHandlerFactory.cs`: Factory per la risoluzione dinamica degli handler dei messaggi.
* `Kafka/OrdineCreatoHandler.cs`: Implementazione della logica che aggiorna le quantità a magazzino in risposta a `OrderCreatedEvent`.


* **`Magazzino.Repository`**: Accesso al database Magazzino (entità, dbcontext e framework di persistenza).
* **`Magazzino.Shared`**: Definizioni DTO (cartella `DTOs`) condivise per esporre modelli senza svelare le entità di database.
* **`Magazzino.ClientHttp`**: Libreria per le comunicazioni HTTP inter-servizio dirette al Magazzino.

### 5. API Gateway (`src/Gateway/Ecommerce.Gateway`)

Il punto di accesso unico per tutti i client esterni, che protegge la rete interna e in strada le richieste.

* **Configurazione e Avvio**: Contiene il `Program.cs`, il `Dockerfile` e il file `Ecommerce.Gateway.http` per i test diretti.
* **Routing**: Sfrutta la libreria `Yarp.ReverseProxy`. Le regole di instradamento per inoltrare il traffico dal Gateway ai microservizi di backend sono definite nei file `appsettings.json` e `appsettings.Development.json`.

### 6. Frontend Applicativo (`src/Frontend/Ecommerce.Frontend`)

Applicazione client web per l'interazione degli utenti finali e dei fornitori, interamente containerizzata.

* **Infrastruttura**: Viene esposta e pacchettizzata tramite il suo `Dockerfile` dedicato (tipicamente basato su Nginx).
* **Pagine principali (HTML)**:
* `index.html`: Pagina pubblica o dashboard principale.
* `login.html`: Schermata di autenticazione per l'acquisizione del token JWT.


* **Aree Riservate**:
* `cliente/ordini.html`: Interfaccia lato utente per visualizzare ed effettuare ordini.
* `fornitore/prodotti.html`: Dashboard riservata ai fornitori per consultare le richieste di rifornimento e i cataloghi.


* **Logica e Stile**:
* `js/api.js`: Modulo JavaScript centralizzato per instradare le richieste HTTP verso il Gateway.
* `js/auth.js`: Modulo per la gestione del salvataggio e invio del token JWT negli header delle richieste.
* `css/style.css`: Foglio di stile globale.



---

## ⚙️ Flussi di Comunicazione Dettagliati (Event-Driven via Kafka)

Il sistema implementa una coreografia di eventi per mantenere sincronizzati i domini senza chiamate HTTP bloccanti. Un flusso end-to-end dimostrabile dal codice sorgente:

1. **Creazione Ordine**: Viene scaturito un evento `OrderCreatedEvent` (definito in `EventBus.Contracts`).
2. **Aggiornamento Giacenza (Magazzino)**: Il microservizio Magazzino è in ascolto. Il routing di Kafka innesca `MagazzinoMessageHandlerFactory.cs`, che indirizza il messaggio a `OrdineCreatoHandler.cs`. Il Magazzino scala il prodotto nel database.
3. **Segnalazione Sottoscorta (Magazzino)**: Se l'operazione precedente abbassa la quantità sotto la soglia di sicurezza, il Magazzino pubblica l'evento `ScortaBassaEvent` (definito in `Approvvigionamento.Shared`).
4. **Richiesta Fornitura (Approvvigionamento)**: Il microservizio Approvvigionamento riceve il messaggio tramite `ScortaBassaConsumer.cs` (nella WebApi). Questo innesca il `FornituraService.cs` (nella Business logic) che registra un record `RichiestaRifornimento.cs` nel proprio database dedicato.

---

## 🚀 Istruzioni per l'Avvio in Sviluppo Locale

### Prerequisiti

* .NET 10 SDK installato.
* Docker Desktop e Docker Compose operativi.
* Un editor/IDE come Visual Studio 2022 o JetBrains Rider.

### Avviare l'Intero Sistema tramite Docker

Il file `docker-compose.yml` è già configurato per creare tutte le immagini e montare i volumi necessari:

```bash
# Esegui il build e avvia i container in background
docker-compose up -d --build

```

Questo comando avvierà:

* Tutti i database PostgreSQL/MySQL (eseguendo i file in `database/*.sql`).
* Il cluster Apache Kafka (Zookeeper e Broker).
* Il Reverse Proxy YARP.
* I microservizi Magazzino e Approvvigionamento.
* L'interfaccia statica Frontend.

### Avviare parzialmente per lo sviluppo su IDE

Per sviluppare il codice .NET ed eseguire il debug tramite il file `Ecommerce.slnx`, avvia soltanto i servizi infrastrutturali:

docker-compose up -d kafka zookeeper postgres-magazzino postgres-approvvigionamento postgres-utenti postgres-ordini

Successivamente apri la solution nell'IDE ed esegui i progetti `.WebApi` e `.Gateway` tramite i profili definiti in `Properties/launchSettings.json`.
