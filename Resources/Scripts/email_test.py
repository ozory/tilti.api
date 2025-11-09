import smtplib
from email.mime.text import MIMEText
from email.mime.multipart import MIMEMultipart

# Configurações do email - ALTERE ESTAS CONFIGURAÇÕES
smtp_server = "smtp.gmail.com"
smtp_port = 587
sender_email = "paulodias99@gmail.com"  # Altere para seu email
password = "hcyg eaun kvtz gdmh"    # Altere para sua senha de app
receiver_email = "paulodias99@gmail.com"  # Altere para o email de destino

# Criando a mensagem
message = MIMEMultipart()
message["From"] = sender_email
message["To"] = receiver_email
message["Subject"] = "Teste de Email SMTP"

# Corpo do email
body = "Este é um email de teste para verificar a configuração SMTP."
message.attach(MIMEText(body, "plain"))

try:
    # Criando a conexão SMTP
    print(f"Conectando ao servidor {smtp_server}:{smtp_port}...")
    server = smtplib.SMTP(smtp_server, smtp_port)
    server.set_debuglevel(1)  # Habilita logs detalhados
    
    # Iniciando TLS
    print("Iniciando TLS...")
    server.starttls()
    
    # Login
    print("Tentando login...")
    server.login(sender_email, password)
    
    # Enviando email
    print("Enviando email...")
    text = message.as_string()
    server.sendmail(sender_email, receiver_email, text)
    print("Email enviado com sucesso!")

except Exception as e:
    print(f"Erro ao enviar email: {str(e)}")
    print(f"Tipo do erro: {type(e)}")
finally:
    try:
        server.quit()
        print("Conexão fechada")
    except:
        pass

# Instruções de uso:
"""
Para usar este script:

1. Se estiver usando Gmail:
   - Ative a verificação em duas etapas: https://myaccount.google.com/security
   - Gere uma senha de app: https://myaccount.google.com/apppasswords
   - Use a senha de app gerada no lugar da sua senha normal

2. Altere as configurações no início do script:
   - sender_email: seu email
   - password: sua senha (ou senha de app para Gmail)
   - receiver_email: email do destinatário

3. Execute o script:
   python email_test.py

O script mostrará logs detalhados da comunicação SMTP, ajudando a identificar
onde exatamente está ocorrendo o erro de autenticação.
"""