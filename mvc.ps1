docker image build -t catalog . -f .\Services\Catalog\Catalog.API\Dockerfile
docker tag catalog registry.heroku.com/ibookstore-catalog/web
docker push registry.heroku.com/ibookstore-catalog/web
heroku container:release web -a ibookstore-catalog
