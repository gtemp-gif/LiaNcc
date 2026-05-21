-- Migrazione da Bootstrap Icons a Material Symbols
-- Eseguire questo script per convertire i valori esistenti nel database

-- 1. Servizi
UPDATE dbo.Services
SET Icon = CASE Icon
    WHEN 'bi-airplane' THEN 'flight'
    WHEN 'bi-map' THEN 'map'
    WHEN 'bi-briefcase' THEN 'business_center'
    WHEN 'bi-heart' THEN 'favorite'
    WHEN 'bi-car' THEN 'directions_car'
    WHEN 'bi-taxi' THEN 'local_taxi'
    WHEN 'bi-bus' THEN 'airport_shuttle'
    WHEN 'bi-phone' THEN 'phone'
    WHEN 'bi-envelope' THEN 'mail'
    WHEN 'bi-calendar' THEN 'calendar_month'
    WHEN 'bi-person' THEN 'person'
    WHEN 'bi-people' THEN 'groups'
    WHEN 'bi-globe' THEN 'language'
    WHEN 'bi-image' THEN 'image'
    WHEN 'bi-clock' THEN 'schedule'
    WHEN 'bi-star' THEN 'star'
    WHEN 'bi-check' THEN 'check_circle'
    WHEN 'bi-x' THEN 'close'
    WHEN 'bi-trash' THEN 'delete'
    WHEN 'bi-pencil' THEN 'edit'
    WHEN 'bi-plus' THEN 'add'
    WHEN 'bi-eye' THEN 'visibility'
    WHEN 'bi-gear' THEN 'settings'
    WHEN 'bi-wifi' THEN 'wifi'
    ELSE Icon
END
WHERE Icon LIKE 'bi-%';

-- 2. Caratteristiche Veicoli (Features)
UPDATE dbo.VehicleFeatures
SET Icon = CASE Icon
    WHEN 'bi-airplane' THEN 'flight'
    WHEN 'bi-map' THEN 'map'
    WHEN 'bi-briefcase' THEN 'business_center'
    WHEN 'bi-heart' THEN 'favorite'
    WHEN 'bi-car' THEN 'directions_car'
    WHEN 'bi-taxi' THEN 'local_taxi'
    WHEN 'bi-bus' THEN 'airport_shuttle'
    WHEN 'bi-phone' THEN 'phone'
    WHEN 'bi-envelope' THEN 'mail'
    WHEN 'bi-calendar' THEN 'calendar_month'
    WHEN 'bi-person' THEN 'person'
    WHEN 'bi-people' THEN 'groups'
    WHEN 'bi-globe' THEN 'language'
    WHEN 'bi-image' THEN 'image'
    WHEN 'bi-clock' THEN 'schedule'
    WHEN 'bi-star' THEN 'star'
    WHEN 'bi-check' THEN 'check_circle'
    WHEN 'bi-x' THEN 'close'
    WHEN 'bi-trash' THEN 'delete'
    WHEN 'bi-pencil' THEN 'edit'
    WHEN 'bi-plus' THEN 'add'
    WHEN 'bi-eye' THEN 'visibility'
    WHEN 'bi-gear' THEN 'settings'
    WHEN 'bi-wifi' THEN 'wifi'
    ELSE Icon
END
WHERE Icon LIKE 'bi-%';

-- 3. Tour Info Items
UPDATE dbo.TourInfoItems
SET Icon = CASE Icon
    WHEN 'bi-airplane' THEN 'flight'
    WHEN 'bi-map' THEN 'map'
    WHEN 'bi-briefcase' THEN 'business_center'
    WHEN 'bi-heart' THEN 'favorite'
    WHEN 'bi-car' THEN 'directions_car'
    WHEN 'bi-taxi' THEN 'local_taxi'
    WHEN 'bi-bus' THEN 'airport_shuttle'
    WHEN 'bi-phone' THEN 'phone'
    WHEN 'bi-envelope' THEN 'mail'
    WHEN 'bi-calendar' THEN 'calendar_month'
    WHEN 'bi-person' THEN 'person'
    WHEN 'bi-people' THEN 'groups'
    WHEN 'bi-globe' THEN 'language'
    WHEN 'bi-image' THEN 'image'
    WHEN 'bi-clock' THEN 'schedule'
    WHEN 'bi-star' THEN 'star'
    WHEN 'bi-check' THEN 'check_circle'
    WHEN 'bi-x' THEN 'close'
    WHEN 'bi-trash' THEN 'delete'
    WHEN 'bi-pencil' THEN 'edit'
    WHEN 'bi-plus' THEN 'add'
    WHEN 'bi-eye' THEN 'visibility'
    WHEN 'bi-gear' THEN 'settings'
    WHEN 'bi-wifi' THEN 'wifi'
    ELSE Icon
END
WHERE Icon LIKE 'bi-%';
