# csharp_simulator

Simulator which is a part of a university thesis.

Planned architecture:

### C# part

- YAML Reader
    - calls dll for TLE parsing
- Moon object
    - calls dll for position of moon
    - properties
        - PositionX
        - PositionY
        - PositionZ
        - Size
- Sun object
    - calls dll for position of sun
    - properties
        - PositionX
        - PositionY
        - PositionZ
        - Size
- Earth object
    - always in the middle, no rotation
    - properties:
        - Size
- Satellite object
    - calls dll for:
        - orbital propagation
        - orbit adjustment using impulse burns
    - properties:
        - Tle line 1
        - Tle line 2
        - mass [kg]
        - thrusterPower → treat as **thrust magnitude [N]** (force)
        - impulse → treat as **Isp [s]**
        - size_* are just metadata for Unity visuals; the DLL doesn’t need them.
        - path is Unity-only; ignore in the DLL layer.
- SimulationNumerical
    - calls Satellite object for:
        - orbit propagation numerical
        - orbit adjustment via impulse burns
    - can have multiple satellites at once
    - calls celestial body simulator only for

### DLL

- uses CSPICE toolkit
- frame and time: **J2000/EME2000** frame, time input as **UTC**
- Earth fixed at origin
- **Forces included:**
    - Central gravity:
    - Optional J2 (one switch/flag)
    - Third-body (Sun, Moon)
    - Thrust: constant force vector  during a scheduled burn