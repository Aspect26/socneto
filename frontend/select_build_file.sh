if test -z "$FRONTEND_ENV"
then
   echo "Using default build file"
else
   BUILD_FILE="./build.${FRONTEND_ENV}.yaml"
   rm "./build.yaml"
   mv "${BUILD_FILE}" "./"
fi
