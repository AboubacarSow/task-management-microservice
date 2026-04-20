FROM docker.elastic.co/beats/filebeat:8.13.0

USER root

COPY filebeat.yml /usr/share/filebeat/filebeat.yml
RUN chmod go-w /usr/share/filebeat/filebeat.yml \
    && chown root:filebeat /usr/share/filebeat/filebeat.yml

USER filebeat