import React from "react";
import { render, screen, fireEvent, waitFor } from "@testing-library/react";
import OrderAanmakenModal from "../components/OrderAanmakenModal";
import "@testing-library/jest-dom";

// Mock fetch globally
global.fetch = jest.fn(() =>
  Promise.resolve({ ok: true, json: () => Promise.resolve({}) })
) as jest.Mock;

const klanten = [{ id: 1, bedrijfsNaam: "Klant A" }];
const producten = [{ id: 101, productName: "Product X" }];

describe("OrderAanmakenModal", () => {
  const mockOnClose = jest.fn();
  const mockOnSuccess = jest.fn();

  beforeEach(() => {
    jest.clearAllMocks();
    render(
      <OrderAanmakenModal
        onClose={mockOnClose}
        onSuccess={mockOnSuccess}
        klanten={klanten}
        producten={producten}
      />
    );
  });

  it("laat validatiefouten zien als vereiste velden ongeldig zijn", async () => {
    fireEvent.change(screen.getByLabelText(/aantal/i), { target: { value: "0" } });
    fireEvent.change(screen.getByLabelText(/verzendadres/i), { target: { value: "" } });
    fireEvent.change(screen.getByLabelText(/verwachte leverdatum/i), {
      target: { value: "2000-01-01" }, // Verleden
    });

    fireEvent.click(screen.getByRole("button", { name: /aanmaken/i }));

    expect(await screen.findByText(/aantal moet een geheel getal van 1 of meer zijn/i)).toBeInTheDocument();
    expect(screen.getByText(/verzendadres mag niet leeg zijn/i)).toBeInTheDocument();
    expect(screen.getByText(/verwachte leverdatum mag niet in het verleden liggen/i)).toBeInTheDocument();
  });

  it("stuurt correcte data bij geldige invoer", async () => {
    const today = new Date().toISOString().split("T")[0];

    fireEvent.change(screen.getByLabelText(/aantal/i), { target: { value: "5" } });
    fireEvent.change(screen.getByLabelText(/verzendadres/i), { target: { value: "Teststraat 123" } });
    fireEvent.change(screen.getByLabelText(/verwachte leverdatum/i), {
      target: { value: today },
    });
    fireEvent.change(screen.getByLabelText(/verwachte levertijd/i), {
      target: { value: "14:30" },
    });

    fireEvent.click(screen.getByRole("button", { name: /aanmaken/i }));

    await waitFor(() => {
      expect(global.fetch).toHaveBeenCalledWith(
        "http://localhost:5000/api/order",
        expect.objectContaining({
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: expect.stringContaining("Teststraat 123"),
        })
      );
      expect(mockOnSuccess).toHaveBeenCalled();
      expect(mockOnClose).toHaveBeenCalled();
    });
  });

  it("roept onSuccess niet aan als de API faalt", async () => {
    (global.fetch as jest.Mock).mockResolvedValueOnce({ ok: false });

    const today = new Date().toISOString().split("T")[0];

    fireEvent.change(screen.getByLabelText(/aantal/i), { target: { value: "2" } });
    fireEvent.change(screen.getByLabelText(/verzendadres/i), { target: { value: "Straatweg 5" } });
    fireEvent.change(screen.getByLabelText(/verwachte leverdatum/i), {
      target: { value: today },
    });
    fireEvent.change(screen.getByLabelText(/verwachte levertijd/i), {
      target: { value: "10:00" },
    });

    fireEvent.click(screen.getByRole("button", { name: /aanmaken/i }));

    await waitFor(() => {
      expect(mockOnSuccess).not.toHaveBeenCalled();
      expect(mockOnClose).not.toHaveBeenCalled();
    });
  });
});
