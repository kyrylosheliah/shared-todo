#include <drogon/drogon.h>
#include "TaskStorePlugin.h"

int main() {
    drogon::app()
        .loadConfigFile("config.json")
        .setLogLevel(trantor::Logger::kDebug)
        .addListener("0.0.0.0", 5000)
        .run();
}
