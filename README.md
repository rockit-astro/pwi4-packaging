This repository contains packaging scripts for PlaneWave Instrument's PWI4 control software.

Planewave host Windows installers and Linux tar bundles at https://planewave.com/files/software/PWI4/

A tar bundle for 4.0.10 was not available at the time of latest update, so we repackage the windows files matching the layout of the previous linux releases.

Binary patches are applied to improve system integration:

* The `~/'Planewave Instruments'` data directory is changed to `/var/opt/pwi4`.

The package depends on a recent version of mono, which can be obtained by adding the upstream repository https://www.mono-project.com/download/stable/#download-lin-centos
