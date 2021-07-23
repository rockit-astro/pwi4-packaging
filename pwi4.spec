%global __provides_exclude_from ^/opt/*
%global __requires_exclude_from ^/opt/*

Name:      pwi4
Version:   4.0.10
Release:   0
Url:       https://github.com/warwick-one-metre/pwi4
Summary:   PlaneWave telescope control software repackaged for CentOS.
License:   Proprietary
Group:     Unspecified
BuildArch: noarch
Requires:  mono, mono-basic
BuildRequires: mono

%description

PlaneWave telescope control software repackaged for CentOS.

%build

mkdir -p %{buildroot}%{_bindir}
mkdir -p %{buildroot}/opt
mkdir -p %{buildroot}/var/opt/pwi4

tar xf %{_sourcedir}/pwi-4.0.10.tar.gz -C %{buildroot} --strip-components=1
mv %{buildroot}/app %{buildroot}/opt/pwi4
rm %{buildroot}/run-pwi4

# Patch "~/PlaneWave Instruments" data directory to /var/opt/pwi4
csc %{_sourcedir}/patch.cs -warn:4 -warnaserror -r:"$(ls /usr/lib/mono/gac/Mono.Cecil/0.11.*/Mono.Cecil.dll)" -out:"patch.exe"
ls %{buildroot}/opt/pwi4/PWLib.dll
mono --debug patch.exe %{buildroot}/opt/pwi4/PWLib.dll %{buildroot}/opt/pwi4/PWLib.dll.temp /var/opt/pwi4
mv %{buildroot}/opt/pwi4/PWLib.dll.temp %{buildroot}/opt/pwi4/PWLib.dll
rm patch.exe

%{__install} %{_sourcedir}/pwi4 %{buildroot}%{_bindir}

%files
%defattr(0755,root,root,0755)
/opt/pwi4/*
%{_bindir}/pwi4

%defattr(0755,root,root,0777)
/var/opt/pwi4

%changelog
