##
##

# Generate a Client Key and a Signing Request
openssl req -config ./config/openssl-client.cnf -newkey rsa:2048 -sha256 -out ../cert/QBisClient.csr -keyout ../cert/QBisClient.key -outform PEM

# Sign the Client CSR with your local CA
openssl ca -config ./config/openssl-ca.cnf -policy signing_policy -extensions signing_req -out ../cert/QBisClient.crt -infiles ../cert/QBisClient.csr 

# Create .pfx
openssl pkcs12 -export -out ../cert/QBisClient.pfx -inkey ../cert/QBisClient.key -in ../cert/QBisClient.crt -certfile ../cert/CA.crt

