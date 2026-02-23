# SystemIntegration Lektion 5

## Opgave 1

Start projektet, der skulle gerne starte en konsol applikation, der venter på indkommende order.
Derudover skulle der også åbnes en webbroweser til scalar med et enkelt endpoint api/orders. 

Test dette endpont

{
  "id": null,
  "destination": "",
  "quickOrder": true,
  "pickUpTime": null
}

id - bliver genereret af web api'et så det skal I ikke sætte. 
destination - er en hvor kunden skal samles op.
quickOrder - om bestillingen er snarest muligt. 
pickUpTime - hvis quickOrder er sat til false, angives et tidspunkt med formattet YYYY-MM-DDTHH:MM:SS, eksempel 2026-02-24T12:15:00

---

## Opgave 2

Lige nu bliver order der bliver accepteret bare fjernet fra listen over order i 
GetUserInput() metoden. I skal implementere et request/reply mønster, hvor der bliver sendt en besked til web api applikationen, når en order bliver accepteret.
På serveren (web api) skal orderen fjernes fra 

```csharp

 public DbSet<Order> Orders { get; set; }

 ```

 og et accept skal sendes tilbage til konsol applikationen, hvis orderen stadig er tilgængelig. Hvis orderen ikke er tilgængelig, skal der sendes en fejlbesked tilbage til konsol applikationen.

 Først når konsol applikationen modtager en accept besked, skal orderen fjernes fra listen over order i GetUserInput() metoden.

I finder et Client/Server eksempel i Examples folderen, som I kan bruge som inspiration til at implementere dette.
