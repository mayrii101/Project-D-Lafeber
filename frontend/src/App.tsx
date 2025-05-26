// src/App.tsx
import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Dashboard from "./Dashboard";
import Bestellingen from "./Bestellingen";
import Producten from "./Producten";
import Zendingen from "./Zendingen";

const App: React.FC = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<Dashboard />} />
        <Route path="/bestellingen" element={<Bestellingen />} />
        <Route path="/Producten" element={<Producten />} />
        <Route path="/Zendingen" element={<Zendingen />} />
      </Routes>
    </Router>
  );
};

export default App;
