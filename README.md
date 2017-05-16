![alt tag](http://i.imgur.com/rWDvl5n.png)
# KSP_MOCR
A standalone program that connects to a kRPC-server, running on , and displays spacecraft/flight/orbital data

Ascent screen:
![alt tag](http://i.imgur.com/KO8Et8M.png)

Pilot screen:
![alt tag](http://i.imgur.com/Tk9I864.png)

To set up the FDAI for launch type the following into the DSKY:
```
VERB 26 NOUN 20 ENTR
+00000 ENTR
+00000 ENTR
-09000 ENTR
```
The FDAI will be upside down, because this setup is based on a heads-down pitch-over manouver at due east.

If you want the FDAI the "right side up", with a a 72° launch angle do this:
```
VERB 26 NOUN 20 ENTR
+18000 ENTR
+00000 ENTR
-07200 ENTR
```

To set up the FDAI for a standard KSP "lean right" ascent:
```
VERB 26 NOUN 20 ENTR
+00000 ENTR
+00000 ENTR
-09000 ENTR
```

As it stands, the programs has the following screens:
* Ascent
* Booster
* Resources
* FDI (Orbital maneuver planning)
* Altitude/Terrain-Height Graph
* Altitude/Speed Plot
* Pilot-mode, for controlling the spacecraft, with _very_ simple DSKY

## Pilot DSKY
Keyboard shortcuts for the DSKY are as follows:
```
ENTR - [E] (Sometimes the Enter/Return button works also. There is a bug somewhere.)
VERB - [V]
NOUN - [N]
CLR  - [C] (this clears the current active input, to correct any typing errors)
 +   - [+]
 -   - [-]
0-9  - [0]-[9]
```

The DSKY in the pilot-screen has the following VERBs and NOUNs implemented:

#### VERBS
```
* 16 - Monitor Noun in R1 or R1,R2 or R1,R2,R3
* 21 - Load component 1 into R1
* 26 - Load component 1,2,3 into R1,R2,R3
* 69 - Clear DSKY displays
```

#### NOUNS
```
* 17 - XXX.XX deg - Current vessel angles R,P,Y (Surface Reference)
* 18 - XXX.XX deg - Current vessel angles R,P,Y (Body Non Rotating Reference)
* 19 - XXX.XX deg - Current angles on FDAI
* 20 - XXX.XX deg - FDAI offset angles
* 29 - XXX.XX deg - Launch azimuth
* 36 - 000XX. hrs - Mission Elapsed Time
       000XX. min
       0XX.XX sec
* 44 - XXX.XX km - Apogee
       XXX.XX km - Perigee
       XXX.XX km - Current Altitude
* 73 - XXX.XX km - Altitude
       XXXX.X m/s - Orbital speed
       XXX.XX deg - Pitch angle
```
### DSKY Examples
#### Monitor current apogee, perigee and altitude:
```
VERB 16 NOUN 44 ENTR
```
## FIDO Screen for calculating manouvers
![alt tag](http://i.imgur.com/U8lwVSS.png)
