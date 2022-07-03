namespace XORCipher_FS
open System

type public XORCipher() =
    static member GetRandomKey(key: int, length:int) =
        let mutable gamma = ""
        let rnd = Random(key)
        for i = 0 to length do gamma <- gamma + string(char(rnd.Next(35, 126)))
        gamma
    
    member private this.GetRepeatKey(s:string) (n:int) =
        let mutable r = s
        while r.Length < n do r <- r + r
        r.Substring(0, n)
    
    member private this.Cipher(text: string) (secretKey: string) =
        let currentKey = this.GetRepeatKey secretKey text.Length
        let mutable res = ""
        for i = 0 to text.Length - 1 do res <- res + string(char(int(text[i]) ^^^ int(currentKey[i])))
        res
        
    member public this.Encrypt(plainText: string, password: string) = this.Cipher plainText password
        
    member public this.Decrypt(encryptedText: string, password: string) = this.Cipher encryptedText password