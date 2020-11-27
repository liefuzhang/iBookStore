docker image build -t usermanagement . -f .\Services\userManagement\userManagement.API\Dockerfile
docker tag usermanagement registry.heroku.com/ibookstore-usermanagement/web
docker push registry.heroku.com/ibookstore-usermanagement/web
heroku container:release web -a ibookstore-usermanagement
