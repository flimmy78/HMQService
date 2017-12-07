sc stop "HMQServiceDaemon"
taskkill /im HMQServiceDaemon.exe /f /t
sc stop "HMQService"
taskkill /im HMQService.exe /f /t

