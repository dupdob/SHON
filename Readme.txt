Welcome to SHON
---------------

SHON is an open source generic (.Net) service host. It is currently in its alpha phase, with a first beta being build.

Main use cases are:
- use for medium to large scale infrastructure based on Windows Service
- ease deployment, upgrade and monitoring
- can be used for isolated service as well

Drivers are:
- decoupling .Net service from the SCM (Windows Service Control Manager) infrastructure to ease deployment
- provide a minimal set of features that eases operations and support such as error handling, configuration options, logging
- permit upgrades through simple file copy
- loose coupling between Host executable and guest service (based on reflection)



Cyrille Dupuydauby
