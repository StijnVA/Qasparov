README
======
All certificates in this solution are for test purpose. Since the private key is public available they can not be trusted for any purpose other than development. 

To make the authentication work, you need to add the CA.crt to your local Trust store
(Using certmgr on Linux) ==> certmgr -add -c -m Trust CA.crt