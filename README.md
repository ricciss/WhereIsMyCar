# Where is my car (Spēle)

Unity 2D spēle — jāsavelk transportlīdzekļi  uz pareizajām vietām pilsētas kartē. Pareizi nometot, atskan skaņa un objekts piestrādā vietā.

**Galvenās funkcijas:** vilkšana/nomešana, rotācija un mērogošana , nejauša objektu izvietošana katrā spēlē, kameras tuvinājums.

Projektā ir arī otra spēle — **Hanojas tornis** (skat. zemāk).

## Kontroles
* Bultiņas - mainīt izmēru
* Z/X - Rotēt
* Space - Apgriezt pretēji
* Scroll - tuvināt ekrānu

## Hanojas tornis

Klasiskā loģikas mīkla, kas pieejama no galvenās izvēlnes. Uzdevums ir pārvietot visus diskus no pirmā stieņa uz pēdējo (trešo) stieni pēc iespējas ātrāk.

### Noteikumi
* Vienlaikus drīkst pārvietot tikai **vienu disku**, un vilkt var tikai **augšējo** disku uz stieņa.
* **Lielāku disku nedrīkst likt uz mazāka.** Nepareizs gājiens atskaņo kļūdas skaņu, un disks atlec atpakaļ.
* Spēle sākas, kad diski ir sakrauti uz pirmā stieņa (lielākais apakšā, mazākais augšā).
* Uzvara, kad visi diski ir uz pēdējā stieņa.

### Kontroles
* Pele / pirksts — vilkt un nomest diskus uz stieņiem
* Restart poga — sākt kārtu no jauna
* Return poga vai Esc — atgriezties galvenajā izvēlnē

### Taimeris un zvaigznes
Taimeris sākas ar pirmo gājienu. Uzvaras logā tiek parādīts laiks un zvaigznes:

| Laiks | Zvaigznes |
|-------|-----------|
| līdz 150 s | 3 |
| līdz 180 s | 2 |
| ilgāk | 1 |

Laika sliekšņus un disku skaitu (3–6) var mainīt Unity inspektorā (`HanoiGameManager`).

### Reklāmas
* Reward reklāmu var noskatīties **vienu reizi kārtā** — par to no laika tiek noņemtas 30 sekundes.
* Baneris tiek rādīts ekrāna augšā, un tas tiek paslēpts uz pilnekrāna reklāmu laiku.

### Ekrānuzņēmumi

<!-- Velc bildes uz šo failu GitHub redaktorā, un GitHub pats ieliks saiti -->

**Spēles skats**

<img width="734" height="351" alt="image" src="https://github.com/user-attachments/assets/c0909abe-d178-4371-8ff6-dc12aacf5229" />


**Uzvaras logs**

<img width="730" height="345" alt="image" src="https://github.com/user-attachments/assets/41b16011-1bac-4e66-834a-4b364adbfdf0" />

## To-Do

- [x] Vilkšanas/nomešanas (drag & drop) mehānika
- [x] Pareizības pārbaude pēc pozīcijas, rotācijas un izmēra
- [x] Skaņu atskaņošana par pareizu/nepareizu novietojumu katram transportlīdzeklim
- [x] Nejauša transportlīdzekļu un vietu izvietošana katras spēles sākumā
- [x] Objekta rotācija (Z/X), mērogošana (bulttaustiņi) un spoguļošana (Space)
- [x] Kameras panorāma (pele) un tuvinājums (ritenītis) ar robežām
- [x] Vilkto objektu noturēšana ekrāna redzamajā zonā
- [x] Main menu pievienošana
- [x] Lidmašīna un mākoņi Pievienošana
- [x] Taimeris (sekundes, formāts hh:mm:ss)
- [x] Uzvaras logs ar zvaigžņu sistēmu (0-3 zvaigznes atkarībā no laika un zaudētajām mašīnām)
- [x] Dzīvību sistēma (3 dzīvības, mašīnas uzspridzināšana samazina dzīvību skaitu)
- [x] Game Over logs, ja dzīvības beidzas
- [x] Restart un Return (uz sākuma ainu) pogas
- [x] Iespēja, ka daļa transportlīdzekļu uzspawnojas spoguļattēlā (jāapgriež ar Space)
- [x] Hanojas torņa spēle (diski, stieņi, taimeris, zvaigznes)
- [x] Reklāmas (interstitial, reward, baneris)

## Izmantotās tehnoloģijas

- **Unity Engine** (2D projekts, UI Canvas / uGUI)
- **C#** — spēles loģika un skripti
- **TextMeshPro (TMP)** — teksta attēlošana (taimeris, laiks, dzīvības)
- **Unity Ads** — interstitial, reward un banera reklāmas

## Bildes

<img width="932" height="522" alt="image" src="https://github.com/user-attachments/assets/babe04a8-ac05-41e3-b5f9-de8b82287a78" />

<img width="927" height="527" alt="image" src="https://github.com/user-attachments/assets/2b9eab5d-0a8e-479f-a8a5-1ef7935e4355" />
<img width="1047" height="619" alt="image" src="https://github.com/user-attachments/assets/289dc3db-3694-4fcc-a1f3-9084bccdf39a" />
<img width="1001" height="574" alt="image" src="https://github.com/user-attachments/assets/44093e35-44af-47ee-b9c1-88d22e07f5fd" />
