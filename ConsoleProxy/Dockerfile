# docker build -t registry.steffbeckers.eu/console-proxy/download:1.0.0 .
# docker push registry.steffbeckers.eu/console-proxy/download:1.0.0

FROM nginx:alpine
RUN mkdir -p /usr/share/nginx/html/download/
COPY . /usr/share/nginx/html/download/
