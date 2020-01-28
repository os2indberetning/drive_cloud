angular.module("application").service('File', ["$resource", function ($resource) {
    return $resource("/odata/DriveReports/Service.TransferReportsToPayroll", { id: "@id" }, {
        "generateFileReport": { method: "GET" }
    });
}]);