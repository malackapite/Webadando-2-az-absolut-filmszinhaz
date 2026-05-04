angular.module('catApp')
.factory('OrderModel', function() {
    return {
        
        getSzallitasiModok: function() {
            return [
                { id: 'haz', nev: 'Házhozszállítás', ar: 1500 },
                { id: 'fox', nev: 'FoxPost Automata', ar: 990 },
                { id: 'szem', nev: 'Személyes átvétel', ar: 0 }
            ];
        },

        getFizetesiModok: function() {
            return [
                { id: 'kartya', nev: 'Bankkártya' },
                { id: 'utanvet', nev: 'Utánvét' },
                { id: 'utalas', nev: 'Banki átutalás' }
            ];
        }
    };
});