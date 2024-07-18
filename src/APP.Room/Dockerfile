FROM node:lts-alpine3.17 AS deps

RUN apk add --no-cache libc6-compat
WORKDIR /app


COPY package.json yarn.lock* package-lock.json* pnpm-lock.yaml* ./
RUN \
  if [ -f yarn.lock ]; then yarn --frozen-lockfile; \
  elif [ -f package-lock.json ]; then npm ci; \
  elif [ -f pnpm-lock.yaml ]; then yarn global add pnpm && pnpm i --frozen-lockfile; \
  else echo "Lockfile not found." && exit 1; \
  fi


FROM node:lts-alpine3.17 AS builder

ARG ENVIROMENT

WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY . .

RUN if [ ${ENVIROMENT} = "Production" ]; then \
        mv .env.prod .env && \
        rm .env.dev .env.local  .env.example; \
    fi
RUN if [ ${ENVIROMENT} = "Develop" ]; then \
        mv .env.dev .env && \
        rm .env.prod .env.local  .env.example; \
    fi


RUN npm run build

FROM node:lts-alpine3.17 AS runner
WORKDIR /app

ENV NODE_ENV production

RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs

COPY --from=builder /app/public ./public

COPY --from=builder --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=builder --chown=nextjs:nodejs /app/.next/static ./.next/static

USER nextjs

EXPOSE 3000

ENV PORT 3000

RUN ls
CMD [ "node", "server.js"]
