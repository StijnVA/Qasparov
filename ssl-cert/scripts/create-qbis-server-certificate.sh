##
##

# Generate a Server Key and a Signing Request
openssl req -config ./config/openssl-server.cnf -newkey rsa:2048 -sha256 -out ../cert/QBisServer.csr -keyout ../cert/QBisServer.key -outform PEM

# Sign the Server CSR with your local CA
openssl ca -config ./config/openssl-ca.cnf -policy signing_policy -extensions signing_req -out ../cert/QBisServer.crt -infiles ../cert/QBisServer.csr 

# Create .pfx
openssl pkcs12 -export -out ../cert/QBisServer.pfx -inkey ../cert/QBisServer.key -in ../cert/QBisServer.crt -certfile ../cert/CA.crt

