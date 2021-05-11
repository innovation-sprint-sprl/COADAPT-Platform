#!/root/venv/datacrypt/bin/python3.8
import sys
from Crypto.PublicKey import RSA
from base64 import b64decode
from Crypto.Cipher import PKCS1_OAEP
from Crypto.Signature import PKCS1_v1_5
from Crypto.Hash import SHA512, SHA384, SHA256, SHA, MD5
from Crypto.Cipher import AES
from Crypto.Protocol.KDF import PBKDF2

skip_verification = False
unpad = lambda s: s[:-ord(s[len(s) - 1:])]


def get_private_key(password):
    salt = b"this is the coadapt salt"
    kdf = PBKDF2(password, salt, 64, 1000)
    key = kdf[:32]
    return key


def sym_decrypt(enc, password):
    private_key = get_private_key(password)
    enc = b64decode(enc)
    iv = enc[:16]
    cipher = AES.new(private_key, AES.MODE_CBC, iv)
    return unpad(cipher.decrypt(enc[16:]))


def importKey(externKey):
    return RSA.importKey(open(externKey).read())


def decrypt(ciphertext, priv_key):
    cipher = PKCS1_OAEP.new(priv_key)
    return cipher.decrypt(ciphertext)


def verify(message, signature, pub_key, hash="SHA-512"):
    signer = PKCS1_v1_5.new(pub_key)
    if (hash == "SHA-512"):
        digest = SHA512.new()
    elif (hash == "SHA-384"):
        digest = SHA384.new()
    elif (hash == "SHA-256"):
        digest = SHA256.new()
    elif (hash == "SHA-1"):
        digest = SHA.new()
    else:
        digest = MD5.new()
    digest.update(message)
    return signer.verify(digest, signature)


def receive(encrypted, senderPublicKeyPath, receiverPrivateKeyPath):
    # deconcatenate
    parts = encrypted.split("|")
    # get signature
    signature = b64decode(parts[0])
    # get encrypted password
    encrypted_password = b64decode(parts[1])
    # get encrypted message
    encrypted_message = b64decode(parts[2])
    # decrypt password using BOB private key
    decrypted_password = decrypt(encrypted_password, importKey(receiverPrivateKeyPath))
    if not skip_verification:
        # verify the signature using ALICE public key
        verified = verify(decrypted_password, signature, importKey(senderPublicKeyPath))
        if not verified:
            raise Exception("ERROR: The signature cannot be authenticated.")
    # decrypt message
    decrypted = sym_decrypt(encrypted_message, decrypted_password.decode('ascii'))
    return decrypted


with open(sys.argv[1] + "encrypted.tmp", 'r') as file:
    data = file.read()
data = receive(data, sys.argv[2], sys.argv[3]).decode('utf8')
with open(sys.argv[1] + "decrypted.tmp", 'w') as file:
	file.write(data)
print('OK')
