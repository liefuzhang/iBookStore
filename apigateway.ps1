docker image build -t apigateway . -f .\ApiGateway\Dockerfile
docker tag apigateway registry.heroku.com/ibookstore-apigateway/web
docker push registry.heroku.com/ibookstore-apigateway/web
heroku container:release web -a ibookstore-apigateway
