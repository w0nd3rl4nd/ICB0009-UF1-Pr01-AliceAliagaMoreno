# Simulación de Registro/Login y Envío Seguro de Mensajes

## Índice
1. [Objetivo](#objetivo)  
2. [Estructura del Proyecto](#estructura-del-proyecto)  
3. [Parte 1: Registro y Login con Seguridad](#parte-1-registro-y-login-con-seguridad)  
4. [Parte 2: Simulación de Envío y Recepción Segura](#parte-2-simulación-de-envío-y-recepción-segura)  
5. [Reflexión sobre Métodos Innecesarios](#reflexión-sobre-métodos-innecesarios)  

---

## Objetivo

- **Parte 1:** Gestionar credenciales de usuario (registro/login) con almacenamiento seguro de contraseñas.  
- **Parte 2:** Simular un protocolo de envío/recepción de mensajes usando criptografía asimétrica (RSA) y simétrica (AES).

---

## Estructura del Proyecto


ICB0009-UF1-Pr01-AliceAliagaMoreno/

    ClaveAsimetricaClass/
        Class1.cs
    ClaveSimetricaClass/
        Class1.cs
    SimulacionEnvioRecepcion/
        Program.cs

- **ClaveAsimetricaClass:** Métodos RSA (firmar, verificar, cifrar, descifrar).  
- **ClaveSimetricaClass:** Métodos AES (generar clave/IV, cifrar, descifrar).  
- **Program.cs:** Lógica de registro/login y simulación completa.

---

## Parte 1: Registro y Login con Seguridad

**Objetivo:** Completar el proceso de registro y login almacenando la contraseña de forma segura en memoria.

1. **Variables Globales**  
   - `string UserName`  
   - `string SecurePass`  (hash + salt)  
   - `byte[] Salt`  

2. **Etapa 1 – Registro (2 puntos)**  
   - El usuario indica nombre de usuario → `UserName`.  
   - El usuario indica contraseña en texto plano.  
   - Generamos un **salt** aleatorio (16 bytes).  
   - Concatenamos `salt || contraseña_UTF8`, y calculamos `hash = SHA512(salt || pwd)`.  
   - Guardamos `SecurePass = Base64(hash)` y `Salt`.  

3. **Etapa 2 – Login (2 puntos)**  
   - El usuario introduce `userName` y `password`.  
   - Si `userName != UserName` → rechazo.  
   - Repetimos: `hash2 = SHA512(salt || password)`.  
   - Si `Base64(hash2) == SecurePass` → login exitoso; en caso contrario → rechazo.  


> **[DIB01]**  
> El sistema de registro/login usa SHA512+salt: en el registro se almacena el hash de `salt`, y en el login se recalcula con el mismo salt para verificar.

---

## Parte 2: Simulación de Envío y Recepción Segura

**Objetivo:** Implementar un flujo en 6 pasos que garantice confidencialidad, integridad y autenticidad.

### Lado Emisor

1. **Firmar Mensaje**  
   - Datos: `TextoAEnviar` → `mensajeBytes = UTF8(TextoAEnviar)`.  
   - Firma: `Firma = Emisor.FirmarMensaje(mensajeBytes)`.  

2. **Cifrar Mensaje (AES)**  
   - La clase `ClaveSimetricaEmisor` genera internamente `Key` e `IV`.  
   - Cifrado: `TextoCifrado = ClaveSimetricaEmisor.CifrarMensaje(TextoAEnviar)`.

3. **Cifrar Clave Simétrica (RSA)**  
   - Clave AES: `Key` e `IV`.  
   - Cifrar ambos con la clave pública del receptor:  
     ```csharp
     ClaveSimetricaKeyCifrada = Receptor.CifrarMensaje(ClaveSimetricaEmisor.Key);
     ClaveSimetricaIVCifrada  = Receptor.CifrarMensaje(ClaveSimetricaEmisor.IV);
     ```
4. **“Envío” al receptor**  
   - Se transmiten:  
     - `Firma`  
     - `TextoCifrado`  
     - `ClaveSimetricaKeyCifrada`, `ClaveSimetricaIVCifrada`

### Lado Receptor

5. **Descifrar Clave Simétrica**  
   - Descifrar con la clave privada del receptor:  
     ```csharp
     byte[] keyDesc = Receptor.DescifrarMensaje(ClaveSimetricaKeyCifrada);
     byte[] ivDesc  = Receptor.DescifrarMensaje(ClaveSimetricaIVCifrada);
     ClaveSimetricaReceptor.Key = keyDesc;
     ClaveSimetricaReceptor.IV  = ivDesc;
     ```

6. **Descifrar Mensaje (AES)**  
   - Con la instancia `ClaveSimetricaReceptor`:  
     ```csharp
     string mensajeDesc = ClaveSimetricaReceptor.DescifrarMensaje(TextoCifrado);
     ```
   
7. **Verificar Firma (RSA)**  
   - Recalculamos los bytes del mensaje descifrado y comprobamos:  
     ```csharp
     bool ok = Emisor.ComprobarFirma(Firma, UTF8(mensajeDesc));
     ```
   - Si `ok`, el mensaje es auténtico e íntegro; en caso contrario, alertar.


## Pregunta:
### Una vez realizada la práctica, ¿crees que alguno de los métodos programado en la clase asimétrica se podría eliminar por carecer de una utilidad real?


En `ClaveAsimetricaClass` existen sobrecargas que admiten pasar una clave externa (`RSAParameters ClavePublicaExterna`) tanto para firmar como para verificar.  
- **Uso actual:** Solo se emplean las claves internas de `Emisor` y `Receptor`.  
- **Conclusión:** Las versiones con parámetro externo no aportan valor en esta implementación y podrían eliminarse para simplificar la API.