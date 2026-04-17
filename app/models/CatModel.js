angular.module('catApp')
.factory('CatModel', function($http) {
    let cart = {}; // Struktúra: { macskaId: darabszam }

    return {
        // Adatok betöltése
        loadCats: function() {
            return $http.get("adat.json").then(resp => resp.data.OBJEKTUMLISTA);
        },

        findCatById: function(id) {
            return this.loadCats().then(cats => cats.find(cat => cat.id == id));
        },

        getCart: () => cart,

        deleteCart: () => { cart = {}; },
        
        addToCart: function(id, qty) {
            cart[id] = (cart[id] || 0) + parseInt(qty);
        },
        
        removeItem: function(id) {
            delete cart[id];
        }
    };
});