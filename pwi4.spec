%global __provides_exclude_from ^/opt/*
%global __requires_exclude_from ^/opt/*

Name:      pwi4
Version:   4.0.11.b18
Release:   1
Url:       https://github.com/warwick-one-metre/pwi4
Summary:   PlaneWave telescope control software repackaged for CentOS.
License:   Proprietary
Group:     Unspecified
BuildArch: noarch
Requires:  mono, mono-basic, libcanberra-gtk2
BuildRequires: mono

%description

PlaneWave telescope control software repackaged for CentOS.

%build

mkdir -p %{buildroot}%{_bindir}
mkdir -p %{buildroot}/opt
mkdir -p %{buildroot}/var/opt/pwi4

tar xf %{_sourcedir}/pwi-4.0.11.b18.tar.gz -C %{buildroot} --strip-components=1
mv %{buildroot}/app %{buildroot}/opt/pwi4
rm %{buildroot}/run-pwi4

# Add mount.ha_hours to the /status output
csc %{_sourcedir}/patch_add_ha_status.cs -warn:4 -warnaserror -r:"$(ls /usr/lib/mono/gac/Mono.Cecil/0.11.*/Mono.Cecil.dll)" -out:"patch.exe"
mono --debug patch.exe %{buildroot}/opt/pwi4/PWI4.exe %{buildroot}/opt/pwi4/PWLib.dll %{buildroot}/opt/pwi4/PWI4.exe.tmp
mv %{buildroot}/opt/pwi4/PWI4.exe.tmp %{buildroot}/opt/pwi4/PWI4.exe
rm patch.exe

%{__install} %{_sourcedir}/pwi4 %{buildroot}%{_bindir}

%files
%defattr(0755,root,root,0755)
/opt/pwi4/*
%{_bindir}/pwi4

%defattr(0755,root,root,0777)
/var/opt/pwi4

%changelog
