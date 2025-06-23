%global __provides_exclude_from ^/opt/*
%global __requires_exclude_from ^/opt/*

Name:      pwi4
Version:   4.1.4
Release:   1%{?dist}
Url:       https://github.com/rockit-astro/pwi4
Summary:   PlaneWave telescope control software.
License:   Proprietary
Group:     Unspecified
BuildArch: noarch

%if 0%{?el10} == 0
Requires:  mono mono-basic libcanberra-gtk2
%else
Requires:  mono-core mono-basic
%endif

%description


%build

mkdir -p %{buildroot}%{_bindir}
mkdir -p %{buildroot}/opt
mkdir -p %{buildroot}/var/opt/pwi4

tar xf %{_sourcedir}/pwi-4.1.4final.tar.gz -C %{buildroot} --strip-components=1
mv %{buildroot}/app %{buildroot}/opt/pwi4
rm %{buildroot}/run-pwi4

# Add mount.ha_hours to the /status output
mcs %{_sourcedir}/patch_add_ha_status.cs -warn:4 -warnaserror -r:"$(ls /usr/lib/mono/gac/Mono.Cecil/0.11.*/Mono.Cecil.dll)" -out:"patch.exe"
mono --debug patch.exe %{buildroot}/opt/pwi4/PWI4.exe %{buildroot}/opt/pwi4/PWLib.dll %{buildroot}/opt/pwi4/PWI4.exe.tmp
rm patch.exe

# Add mount.axisN.is_homed to the /status output
mcs %{_sourcedir}/patch_add_homed_status.cs -warn:4 -warnaserror -r:"$(ls /usr/lib/mono/gac/Mono.Cecil/0.11.*/Mono.Cecil.dll)" -out:"patch.exe"
mono --debug patch.exe %{buildroot}/opt/pwi4/PWI4.exe.tmp /usr/lib/mono/4.5/mscorlib.dll %{buildroot}/opt/pwi4/PWI4.exe
rm patch.exe

# Restore ability to call /mount/enable?axis=-1 and /mount/disable?axis=-1 to toggle both axes on/off
mcs %{_sourcedir}/patch_enable_both_axes.cs -warn:4 -warnaserror -r:"$(ls /usr/lib/mono/gac/Mono.Cecil/0.11.*/Mono.Cecil.dll)" -out:"patch.exe"
mono --debug patch.exe %{buildroot}/opt/pwi4/PWI4.exe %{buildroot}/opt/pwi4/PWI4.exe.tmp
rm patch.exe

mv %{buildroot}/opt/pwi4/PWI4.exe.tmp %{buildroot}/opt/pwi4/PWI4.exe
%{__install} %{_sourcedir}/pwi4 %{buildroot}%{_bindir}

%files
%defattr(0755,root,root,0755)
/opt/pwi4/*
%{_bindir}/pwi4

%defattr(0755,root,root,0777)
/var/opt/pwi4

%changelog
