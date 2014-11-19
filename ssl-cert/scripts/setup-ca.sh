##
##

# Generate a private key en self signed certificate for your QBis server

openssl req -x509 -config ./config/openssl-ca.cnf -newkey rsa:4096 -sha256 -out ../cert/CA.crt -outform PEM -keyout ../cert/CA.key

# Add your CA certificate to your local Trust store
certmgr -add -c -m Trust ../cert/CA.crt

