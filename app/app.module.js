angular.module('catApp', ['ngRoute'])
.config(function($routeProvider) {
    $routeProvider
    .when('/login', {
        templateUrl: 'views/login.html',
        controller: 'LoginController'
    })
    .when('/shop', {
        templateUrl: 'views/shop.html',
        controller: 'CatController'
    })
    .when('/checkout', {
        templateUrl: 'views/checkout.html',
        controller: 'StepperController'
    })
    .otherwise({ redirectTo: '/login' });
});