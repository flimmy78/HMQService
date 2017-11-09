sc stop "HMQService"

taskkill /im HMQService.exe /f /t
 
sc delete "HMQService"