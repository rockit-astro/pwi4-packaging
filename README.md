PlaneWave Instrument's PWI4 control software repackaged for Rocky Linux.

Planewave host Windows installers and Linux tar bundles at https://planewave.com/files/software/PWI4/
A tar bundle for the 4.1.4 Linux release is mirrored to ensure future availablility.

Binary patches are applied to improve system integration:

* A mount.ha_hours line is added to the /status http response.

The package depends on a recent version of mono, which can be obtained by adding the upstream repository https://www.mono-project.com/download/stable/#download-lin-centos
