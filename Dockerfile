FROM node:18-alpine

WORKDIR /app

# Копируем package.json сначала (для кэширования)
COPY package*.json ./

# Устанавливаем зависимости
RUN npm ci --only=production

# Копируем остальные файлы
COPY . .

# Открываем порт
EXPOSE 3000

# Запускаем приложение
CMD ["node", "index.js"]"]et", "KeysShop.dll"]