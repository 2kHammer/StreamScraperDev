FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#####################
#PUPPETEER RECIPE based on https://github.com/hardkoded/puppeteer-sharp/issues/1180#issuecomment-1015532968
#####################
RUN apt-get update && apt-get -f install && apt-get -y install wget gnupg2 apt-utils
RUN wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add -
RUN echo 'deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main' >> /etc/apt/sources.list
RUN apt-get update \
&& apt-get install -y google-chrome-stable --no-install-recommends --allow-downgrades fonts-ipafont-gothic fonts-wqy-zenhei fonts-thai-tlwg fonts-kacst fonts-freefont-ttf
######################
#END PUPPETEER RECIPE
######################
ENV PUPPETEER_EXECUTABLE_PATH "/usr/bin/google-chrome-stable"
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build-env
WORKDIR /src
COPY ./StreamScraperTest ./
RUN dotnet restore 

Run dotnet publish -c Release -o /publish 

# Specify image to run the application
FROM base as final
# Working directory for the publish
WORKDIR /publish
# Copy the publish directory from the build stage to the running stage
COPY --from=build-env /publish .
# what do we want to run
ENTRYPOINT ["dotnet", "StreamScraperTest.dll"]