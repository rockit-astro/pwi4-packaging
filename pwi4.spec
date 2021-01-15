%global __provides_exclude_from ^/opt/*
%global __requires_exclude_from ^/opt/*

Name:      pwi4
Version:   4.0.9b9
Release:   0
Url:       https://github.com/warwick-one-metre/pwi4
Summary:   PlaneWave telescope control software repackaged for CentOS.
License:   Proprietary
Group:     Unspecified
BuildArch: noarch
Requires:  mono, mono-basic

%description

PlaneWave telescope control software repackaged for CentOS.

%build

mkdir -p %{buildroot}%{_bindir}
mkdir -p %{buildroot}/opt
tar xf %{_sourcedir}/pwi-4.0.9beta9.tar.gz -C %{buildroot} --strip-components=1
mv %{buildroot}/app %{buildroot}/opt/pwi4
rm %{buildroot}/run-pwi4

%{__install} %{_sourcedir}/pwi4 %{buildroot}%{_bindir}

%files
%defattr(0755,root,root,0755)
/opt/pwi4/*
%{_bindir}/pwi4

%changelog
