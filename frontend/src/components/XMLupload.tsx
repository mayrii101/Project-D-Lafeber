import React, { useState, ChangeEvent, FormEvent } from "react";

function XmlUpload() {
  const [xmlBestand1, setXmlBestand1] = useState<File | null>(null);
  const [xmlBestand2, setXmlBestand2] = useState<File | null>(null);
  const [melding, setMelding] = useState<string>("");
  const [toonPopup, setToonPopup] = useState<boolean>(false);

  const handleBestand1Change = (e: ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) setXmlBestand1(e.target.files[0]);
  };

  const handleBestand2Change = (e: ChangeEvent<HTMLInputElement>) => {
    if (e.target.files) setXmlBestand2(e.target.files[0]);
  };

  const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    if (!xmlBestand1 || !xmlBestand2) {
      setMelding("Selecteer alstublieft beide XML-bestanden.");
      return;
    }

    setToonPopup(true);
  };

  const bevestigUpload = async () => {
    const formData = new FormData();
    formData.append("xmlFile1", xmlBestand1 as File);
    formData.append("xmlFile2", xmlBestand2 as File);

    try {
      const response = await fetch("http://localhost:5000/api/XmlImport/upload", {
        method: "POST",
        body: formData,
      });

      if (response.ok) {
        const result = await response.json();
        setMelding(result.message || "Bestanden succesvol geüpload!");
      } else {
        const errorText = await response.text();
        setMelding(`Upload mislukt: ${errorText}`);
      }
    } catch (error: any) {
      setMelding(`Fout: ${error.message}`);
    } finally {
      setToonPopup(false);
    }
  };

  return (
    <div style={{ maxWidth: "600px", margin: "0 auto", padding: "2rem" }}>
      <h3 style={{ marginBottom: "1.5rem", fontSize: "0.95rem", fontWeight: "500" }}>
        Kies hier de bestanden die u wilt uploaden. Deze bestanden worden geüpload naar de huidige database.
      </h3>

      <form onSubmit={handleSubmit} style={{ display: "flex", flexDirection: "column", gap: "1.5rem" }}>
        <div style={{ display: "flex", gap: "1rem", justifyContent: "space-between" }}>
          <div style={{ flex: 1 }}>
            <label style={{ display: "block", fontWeight: "600", marginBottom: "0.5rem" }}>
              XML-bestand 1:
            </label>
            <label className="custom-file-upload">
              <input type="file" accept=".xml" onChange={handleBestand1Change} />
            </label>
          </div>

          <div style={{ flex: 1 }}>
            <label style={{ display: "block", fontWeight: "600", marginBottom: "0.5rem" }}>
              XML-bestand 2:
            </label>
            <label className="custom-file-upload">
              <input type="file" accept=".xml" onChange={handleBestand2Change} />
            </label>
          </div>
        </div>

        <button type="submit">Uploaden</button>
      </form>

      {melding && <p style={{ marginTop: "1rem", fontWeight: "500" }}>{melding}</p>}

      {/* Popup */}
      {toonPopup && (
        <div style={{
          position: "fixed",
          top: 0, left: 0,
          width: "100%", height: "100%",
          backgroundColor: "rgba(0,0,0,0.5)",
          display: "flex",
          justifyContent: "center",
          alignItems: "center",
          zIndex: 1000,
        }}>
          <div style={{
            backgroundColor: "white",
            padding: "2rem",
            borderRadius: "8px",
            textAlign: "center",
            maxWidth: "400px",
            boxShadow: "0 0 10px rgba(0,0,0,0.25)",
          }}>
            <p style={{ marginBottom: "1rem" }}>Weet u zeker dat u deze bestanden wilt uploaden?</p>
            <div style={{ display: "flex", justifyContent: "center", gap: "1rem" }}>
              <button onClick={bevestigUpload}>Ja, uploaden</button>
              <button onClick={() => setToonPopup(false)}>Annuleren</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default XmlUpload;
