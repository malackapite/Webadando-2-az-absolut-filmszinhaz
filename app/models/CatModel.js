angular.module('catApp')
.factory('CatModel', function($http) {
    let cart = {}; // Struktúra: { macskaId: darabszam }

    return {
        // READ
        loadCats: function() {
            return $http.get("adat.json").then(resp => resp.data.OBJEKTUMLISTA);
        },

        findCatById: function(id) {
            return this.loadCats().then(cats => cats.find(cat => cat.id == id));
        },

        // CREATE
        saveCat: function(newCat) {
            // Valódi backendnél: return $http.post("/api/cats", newCat);
            console.log("Mentés (szimulált):", newCat);
            return Promise.resolve(newCat); 
        },

        // UPDATE
        updateCat: function(cat) {
            // Valódi backendnél: return $http.put("/api/cats/" + cat.id, cat);
            console.log("Módosítás (szimulált):", cat);
            return Promise.resolve(cat);
        },

        // DELETE
        deleteCat: function(id) {
            // Valódi backendnél: return $http.delete("/api/cats/" + id);
            console.log("Törlés (szimulált), ID:", id);
            return Promise.resolve(id);
        },

        getCart: () => cart,

        addToCart: (id, qty) => { cart[id] = (cart[id] || 0) + parseInt(qty); },

        deleteCart: () => { cart = {}; },
        
        removeItem: function(id) {
            delete cart[id];
        },

    };
});