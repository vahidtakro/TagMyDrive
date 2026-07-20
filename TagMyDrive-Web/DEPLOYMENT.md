# TagMyDrive - Deployment Guide

## Table of Contents

1. [System Requirements](#system-requirements)
2. [Database Setup](#database-setup)
3. [Desktop Application Deployment](#desktop-application-deployment)
4. [Web Application Deployment](#web-application-deployment)
5. [Production Checklist](#production-checklist)
6. [Troubleshooting](#troubleshooting)
7. [Monitoring & Maintenance](#monitoring--maintenance)

---

## System Requirements

### Desktop Application (Windows Forms)
- **OS:** Windows 7 SP1 or later
- **.NET Framework:** 4.8 or later
- **RAM:** 512 MB minimum, 2 GB recommended
- **Disk Space:** 100 MB
- **Screen:** 1024x768 minimum resolution
- **Network:** Internet connection for Google Drive integration

### Web Application Server
- **OS:** Linux, Windows Server 2016+, or macOS
- **Node.js:** 16.x or later
- **RAM:** 1 GB minimum, 4 GB recommended
- **Disk Space:** 1 GB
- **Network:** Static IP or domain name

### Database Server
- **MySQL:** 5.7 or later (8.0+ recommended)
- **Storage:** 10 GB initial (scale based on usage)
- **RAM:** 2 GB minimum
- **Network:** Port 3306 open for application servers

---

## Database Setup

### 1. Create Database

```sql
CREATE DATABASE natasun_tagmydrive CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
CREATE USER 'tagmydrive'@'%' IDENTIFIED BY 'tagmydrivecWvzR8s8Inso5';
GRANT ALL PRIVILEGES ON natasun_tagmydrive.* TO 'tagmydrive'@'%';
FLUSH PRIVILEGES;
```

### 2. Import Schema

```bash
mysql -h lv-shared04.cpanelplatform.com -u tagmydrive -p natasun_tagmydrive < schema.sql
```

### 3. Verify Installation

```sql
USE natasun_tagmydrive;
SHOW TABLES;
```

Expected tables: users, memberships, disks, qr_codes, google_drive_links, export_history, user_sessions

### 4. Test Connection

**From Windows:**
```bash
mysql -h lv-shared04.cpanelplatform.com -u tagmydrive -p -e "SELECT VERSION();"
```

**From Linux:**
```bash
mysql -h lv-shared04.cpanelplatform.com -u tagmydrive -p -e "SELECT VERSION();"
```

---

## Desktop Application Deployment

### Development Environment

1. **Clone/Download Project**
   ```bash
   git clone https://github.com/yourusername/TagMyDrive.git
   cd TagMyDrive
   ```

2. **Restore NuGet Packages**
   ```bash
   nuget restore TagMyDrive.sln
   ```

3. **Build Solution**
   ```bash
   msbuild TagMyDrive.sln /p:Configuration=Release /p:Platform="Any CPU"
   ```

4. **Output Location**
   - Release build: `TagMyDrive/bin/Release/`

### Production Deployment - Installer

#### Create MSI Installer (WiX Toolset)

1. **Install WiX Toolset**
   ```bash
   choco install wix -y
   ```

2. **Create WiX Project** (TagMyDrive.Setup.wixproj)

3. **Configure Installer**
   - Set application icon
   - Configure start menu shortcuts
   - Set installation directory: `Program Files\TagMyDrive\`
   - Set uninstall options

4. **Build Installer**
   ```bash
   msbuild TagMyDrive.Setup.wixproj /p:Configuration=Release
   ```

5. **Distribute MSI**
   - Upload to download server
   - Create installer documentation
   - Provide SHA256 hash for verification

### Manual Installation

1. **Create Directory**
   ```bash
   mkdir "C:\Program Files\TagMyDrive"
   ```

2. **Copy Files**
   ```bash
   xcopy bin\Release\* "C:\Program Files\TagMyDrive\" /E /I /Y
   ```

3. **Create Shortcuts**
   ```bash
   @echo off
   setlocal enabledelayedexpansion
   set LNKDIR=%APPDATA%\Microsoft\Windows\Start Menu\Programs\TagMyDrive
   mkdir !LNKDIR!
   powershell -Command "$ws = New-Object -ComObject WScript.Shell; $ws.CreateShortcut('!LNKDIR!\TagMyDrive.lnk').TargetPath = 'C:\Program Files\TagMyDrive\TagMyDrive.exe'; $ws.CreateShortcut('!LNKDIR!\TagMyDrive.lnk').Save()"
   ```

4. **Update Configuration**
   ```xml
   <!-- app.config -->
   <configuration>
	 <connectionStrings>
	   <add name="TagMyDriveDb" 
			connectionString="Server=lv-shared04.cpanelplatform.com;Port=3306;Database=natasun_tagmydrive;Uid=tagmydrive;Pwd=tagmydrivecWvzR8s8Inso5;SslMode=Required;" />
	 </connectionStrings>
   </configuration>
   ```

### Post-Installation

1. **Test Connection**
   - Launch TagMyDrive
   - Test database connectivity
   - Verify user can register and login

2. **Create Desktop Shortcut**
   - Create `.lnk` file pointing to `C:\Program Files\TagMyDrive\TagMyDrive.exe`

3. **Document for Users**
   - Create user guide
   - Setup screen recordings
   - Provide support contact

---

## Web Application Deployment

### Local Development

```bash
cd TagMyDrive-Web
npm install
npm run dev
# Runs on http://localhost:3000
```

### Production Deployment - Vercel (Recommended)

1. **Install Vercel CLI**
   ```bash
   npm install -g vercel
   ```

2. **Configure Project**
   ```bash
   vercel link
   ```

3. **Set Environment Variables**
   ```bash
   vercel env add DB_HOST
   vercel env add DB_PORT
   vercel env add DB_NAME
   vercel env add DB_USER
   vercel env add DB_PASSWORD
   vercel env add JWT_SECRET
   ```

4. **Deploy**
   ```bash
   vercel --prod
   ```

5. **Verify Deployment**
   - Check Vercel dashboard
   - Test endpoints: https://tagmydrive.vercel.app/api/auth/login

### Production Deployment - Self-Hosted (Linux)

1. **Prerequisites**
   ```bash
   sudo apt update
   sudo apt install -y nodejs npm git nginx
   ```

2. **Clone Repository**
   ```bash
   cd /var/www
   git clone https://github.com/yourusername/TagMyDrive-Web.git
   cd TagMyDrive-Web
   ```

3. **Install Dependencies**
   ```bash
   npm install --production
   ```

4. **Build Application**
   ```bash
   npm run build
   ```

5. **Configure Environment**
   ```bash
   cat > .env.production.local << EOF
   DB_HOST=lv-shared04.cpanelplatform.com
   DB_PORT=3306
   DB_NAME=natasun_tagmydrive
   DB_USER=tagmydrive
   DB_PASSWORD=tagmydrivecWvzR8s8Inso5
   JWT_SECRET=$(openssl rand -base64 32)
   NEXT_PUBLIC_API_URL=https://tagmydrive.yourdomain.com
   EOF
   ```

6. **Start Application**
   ```bash
   npm start
   # Runs on http://localhost:3000
   ```

7. **Configure Nginx Reverse Proxy**
   ```nginx
   server {
	   listen 80;
	   server_name tagmydrive.yourdomain.com;

	   # Redirect HTTP to HTTPS
	   return 301 https://$server_name$request_uri;
   }

   server {
	   listen 443 ssl http2;
	   server_name tagmydrive.yourdomain.com;

	   ssl_certificate /etc/letsencrypt/live/tagmydrive.yourdomain.com/fullchain.pem;
	   ssl_certificate_key /etc/letsencrypt/live/tagmydrive.yourdomain.com/privkey.pem;

	   location / {
		   proxy_pass http://localhost:3000;
		   proxy_http_version 1.1;
		   proxy_set_header Upgrade $http_upgrade;
		   proxy_set_header Connection 'upgrade';
		   proxy_set_header Host $host;
		   proxy_cache_bypass $http_upgrade;
	   }
   }
   ```

8. **Setup SSL Certificate (Let's Encrypt)**
   ```bash
   sudo apt install -y certbot python3-certbot-nginx
   sudo certbot certonly --nginx -d tagmydrive.yourdomain.com
   ```

9. **Enable Nginx**
   ```bash
   sudo systemctl enable nginx
   sudo systemctl restart nginx
   ```

10. **Setup Process Manager (PM2)**
	```bash
	sudo npm install -g pm2
	pm2 start npm --name "tagmydrive" -- start
	pm2 startup
	pm2 save
	```

### Production Deployment - Docker

1. **Create Dockerfile**
   ```dockerfile
   FROM node:18-alpine

   WORKDIR /app
   COPY package*.json ./
   RUN npm ci --only=production

   COPY . .
   RUN npm run build

   EXPOSE 3000
   CMD ["npm", "start"]
   ```

2. **Create .dockerignore**
   ```
   .git
   .gitignore
   node_modules
   npm-debug.log
   .next
   out
   .env.local
   ```

3. **Build Image**
   ```bash
   docker build -t tagmydrive:latest .
   ```

4. **Run Container**
   ```bash
   docker run -d \
	 -e DB_HOST=lv-shared04.cpanelplatform.com \
	 -e DB_PORT=3306 \
	 -e DB_NAME=natasun_tagmydrive \
	 -e DB_USER=tagmydrive \
	 -e DB_PASSWORD=tagmydrivecWvzR8s8Inso5 \
	 -e JWT_SECRET=$(openssl rand -base64 32) \
	 -p 3000:3000 \
	 --name tagmydrive \
	 tagmydrive:latest
   ```

---

## Production Checklist

### Security
- [ ] Change all default passwords
- [ ] Enable HTTPS/SSL on web application
- [ ] Configure firewall rules
- [ ] Enable database encryption
- [ ] Setup automated backups
- [ ] Enable database audit logging
- [ ] Change JWT_SECRET to strong random value
- [ ] Disable debug mode
- [ ] Enable CORS restrictions
- [ ] Setup rate limiting

### Performance
- [ ] Enable database connection pooling
- [ ] Setup CDN for static assets
- [ ] Configure caching headers
- [ ] Enable compression (gzip)
- [ ] Optimize images
- [ ] Setup monitoring/alerting
- [ ] Configure database indexes
- [ ] Enable query optimization

### Backup & Recovery
- [ ] Setup daily database backups
- [ ] Test backup restoration
- [ ] Document recovery procedures
- [ ] Store backups off-site
- [ ] Monitor backup success
- [ ] Setup automated backups

### Monitoring
- [ ] Setup error logging (Sentry, LogRocket)
- [ ] Configure uptime monitoring
- [ ] Setup performance monitoring
- [ ] Create dashboards
- [ ] Setup alerts for critical issues
- [ ] Monitor database performance
- [ ] Monitor API response times

### Documentation
- [ ] Document deployment process
- [ ] Document configuration
- [ ] Create runbooks for common tasks
- [ ] Document disaster recovery
- [ ] Create user documentation

---

## Troubleshooting

### Desktop Application

**Issue: "Object reference not set to an instance of an object" on startup**
- Verify database connection string in app.config
- Check if database is accessible
- Ensure schema is properly created

**Issue: Cannot connect to Google Drive**
- Verify OAuth credentials in app.config
- Check internet connectivity
- Ensure Google Drive API is enabled in Google Cloud Console

### Web Application

**Issue: "Cannot find module 'mysql2'"**
```bash
npm install mysql2
```

**Issue: "Connection refused" on database**
- Verify database host and port
- Check firewall rules
- Test database connection: `mysql -h host -u user -p`

**Issue: JWT token invalid**
- Verify JWT_SECRET is set
- Check token expiration (7 days)
- Verify token is being sent in header

---

## Monitoring & Maintenance

### Daily Checks
- [ ] Check application uptime
- [ ] Review error logs
- [ ] Monitor database size
- [ ] Check API response times

### Weekly Checks
- [ ] Review security logs
- [ ] Check backup status
- [ ] Monitor disk usage
- [ ] Review user metrics

### Monthly Checks
- [ ] Review performance metrics
- [ ] Plan for capacity
- [ ] Update dependencies (npm audit)
- [ ] Test disaster recovery

### Quarterly Checks
- [ ] Security audit
- [ ] Load testing
- [ ] Database optimization
- [ ] Documentation review

---

## Rollback Procedure

If deployment fails:

1. **Desktop Application**
   ```bash
   # Restore previous version
   xcopy backup\TagMyDrive-v1.0.1\* "C:\Program Files\TagMyDrive\" /E /I /Y
   ```

2. **Web Application**
   ```bash
   # Vercel automatic rollback
   vercel rollback

   # Manual rollback
   git revert <commit_hash>
   npm run build
   npm start
   ```

---

## Support

For issues or questions:
- Email: support@tagmydrive.com
- GitHub Issues: https://github.com/yourusername/TagMyDrive/issues
- Documentation: https://tagmydrive.com/docs

---

**Last Updated:** January 2024
**Version:** 1.0.0
